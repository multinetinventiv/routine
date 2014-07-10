using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Routine.Core;

namespace Routine.Soa
{
	public class SoaController : Controller
	{
		private readonly ISoaContext context;

		public SoaController(ISoaContext context)
		{
			this.context = context;
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);

			filterContext.ExceptionHandled = true;
			filterContext.Result = LargeJson(context.SoaConfiguration.ExceptionResultExtractor.Extract(filterContext.Exception), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Index()
		{
			var stream = GetType().Assembly.GetManifestResourceStream(
					GetType().Assembly.GetManifestResourceNames().Single(s => s.EndsWith("SoaTestPage.html")));

			var sr = new StreamReader(stream);

			var fileContent = sr.ReadToEnd();
			sr.Close();
			stream.Close();

			fileContent = fileContent
				.Replace("@urlBase", Url.Action("Index"))
				.Replace("@defaultParams", string.Join(",", context.SoaConfiguration.DefaultParameters.Select(s => s.SurroundWith("\""))));

			return File(Encoding.UTF8.GetBytes(fileContent), "text/html");
		}

		public JsonResult GetApplicationModel()
		{
			return LargeJson(context.ObjectService.GetApplicationModel(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetObjectModel(string objectModelId)
		{
			return LargeJson(context.ObjectService.GetObjectModel(objectModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetAvailableObjects(string objectModelId)
		{
			var result = context.ObjectService.GetAvailableObjects(objectModelId);

			return LargeJson(result.Select(o => o.ToSerializable()).ToList(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			return LargeJson(context.ObjectService.GetValue(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			var result = context.ObjectService.Get(reference);

			return LargeJson(result.ToSerializable(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			var result = context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues);

			return LargeJson(result.ToSerializable());
		}

		#region MaxJsonLength extension

		protected LargeJsonResult LargeJson(object data) { return LargeJson(data, JsonRequestBehavior.DenyGet); }
		protected virtual LargeJsonResult LargeJson(object data, JsonRequestBehavior jsonRequestBehavior)
		{
			return new LargeJsonResult
				{
					Data = data,
					MaxJsonLength = context.SoaConfiguration.MaxResultLength,
					JsonRequestBehavior = jsonRequestBehavior
				};
		}

		protected class LargeJsonResult : JsonResult
		{
			private const string JSON_REQUEST_GET_NOT_ALLOWED = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";

			public LargeJsonResult()
			{
				MaxJsonLength = 1024000;
				RecursionLimit = 100;
			}

			public int MaxJsonLength { get; set; }
			public int RecursionLimit { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				if (context == null)
				{
					throw new ArgumentNullException("context");
				}

				if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
					string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException(JSON_REQUEST_GET_NOT_ALLOWED);
				}

				var response = context.HttpContext.Response;

				response.ContentType = ContentType;
				if (string.IsNullOrEmpty(ContentType))
				{
					response.ContentType = "application/json";
				}

				if (ContentEncoding != null)
				{
					response.ContentEncoding = ContentEncoding;
				}

				if (Data == null) { return; }

				var serializer = new JavaScriptSerializer
					{
						MaxJsonLength = MaxJsonLength,
						RecursionLimit = RecursionLimit
					};

				response.Write(serializer.Serialize(Data));
			}
		}

		#endregion
	}
}