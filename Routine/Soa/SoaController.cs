using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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
			filterContext.Result = Json(context.SoaConfiguration.ExceptionResultExtractor.Extract(filterContext.Exception), JsonRequestBehavior.AllowGet);
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
			return Json(context.ObjectService.GetApplicationModel(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetObjectModel(string objectModelId)
		{
			return Json(context.ObjectService.GetObjectModel(objectModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetAvailableObjects(string objectModelId)
		{
			List<ObjectData> result = context.ObjectService.GetAvailableObjects(objectModelId);

			return Json(result.Select(o => o.ToSerializable()).ToList(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			return Json(context.ObjectService.GetValue(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			ObjectData result = context.ObjectService.Get(reference) as ObjectData;

			return Json(result.ToSerializable(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMember(ObjectReferenceData reference, string memberModelId)
		{
			ValueData result = context.ObjectService.GetMember(reference, memberModelId) as ValueData;

			return Json(result.ToSerializable(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues)
		{
			ValueData result = context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues) as ValueData;

			return Json(result.ToSerializable());
		}
	}
}