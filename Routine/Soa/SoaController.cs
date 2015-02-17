using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Soa
{
	public class SoaController : Controller
	{
		public static string ControllerName { get { return typeof(SoaController).Name.BeforeLast("Controller"); } }
		public static string DefaultAction { get { return "Index"; } }

		private const int DEFAULT_RECURSION_LIMIT = 100;

		private static IRestSerializer CreateDefaultSerializer(int maxResultLength)
		{
			return new JsonRestSerializer(new JavaScriptSerializer
				{
					MaxJsonLength = maxResultLength,
					RecursionLimit = DEFAULT_RECURSION_LIMIT
				});
		}

		private readonly ISoaContext context;
		private readonly IRestSerializer serializer;

		public SoaController(ISoaContext context) : this(context, CreateDefaultSerializer(context.SoaConfiguration.GetMaxResultLength())) { }
		public SoaController(ISoaContext context, IRestSerializer serializer)
		{
			this.context = context;
			this.serializer = serializer;
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);

			filterContext.ExceptionHandled = true;
			filterContext.Result = LargeJson(context.SoaConfiguration.GetExceptionResult(filterContext.Exception), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Index()
		{
			var stream = GetType().Assembly.GetManifestResourceStream(
					GetType().Assembly.GetManifestResourceNames().Single(s => s.EndsWith("SoaTestPage.html")));

			if (stream == null) { throw new InvalidOperationException("Could not get manifest resource stream for test page"); }

			var sr = new StreamReader(stream);

			var fileContent = sr.ReadToEnd();
			sr.Close();
			stream.Close();

			fileContent = fileContent
				.Replace("@urlBase", Url.Action("Index"))
				.Replace("@headers", string.Join(",", context.SoaConfiguration.GetHeaders().Select(s => s.SurroundWith("\""))));

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

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			return LargeJson(context.ObjectService.GetValue(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			var result = context.ObjectService.Get(reference);

			return LargeJson(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			var result = context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues);

			return LargeJson(result);
		}

		public JsonResult Perform(ObjectReferenceData targetReference, string operationModelId, string parameters)
		{
			var parameterValues = serializer.Deserialize<Dictionary<string, ParameterValueData>>(parameters);

			return PerformOperation(targetReference, operationModelId, parameterValues);
		}

		#region MaxJsonLength extension

		protected LargeJsonResult LargeJson(object data) { return LargeJson(data, JsonRequestBehavior.DenyGet); }
		protected virtual LargeJsonResult LargeJson(object data, JsonRequestBehavior jsonRequestBehavior)
		{
			return new LargeJsonResult(serializer)
				{
					Data = data,
					JsonRequestBehavior = jsonRequestBehavior
				};
		}

		protected class LargeJsonResult : JsonResult
		{
			private const string JSON_REQUEST_GET_NOT_ALLOWED = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";

			private readonly IRestSerializer serializer;

			public LargeJsonResult(IRestSerializer serializer)
			{
				this.serializer = serializer;
			}

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

				response.Write(serializer.Serialize(Data));
			}
		}

		#endregion
	}
}