using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Routine.Core.Interceptor;
using Routine.Core.Service;
using Routine.Soa.Context;

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

			fileContent = fileContent.Replace("@urlBase", Url.Action("Index"));

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
			return Json(context.ObjectService.GetAvailableObjects(objectModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			return Json(context.ObjectService.GetValue(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			return Json(context.ObjectService.Get(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMember(ObjectReferenceData reference, string memberModelId)
		{
			return Json(context.ObjectService.GetMember(reference, memberModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetOperation(ObjectReferenceData reference, string operationModelId)
		{
			return Json(context.ObjectService.GetMember(reference, operationModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
		{
			var interception = context.SoaConfiguration.PerformOperationInterceptor.Intercept(context.CreatePerformOperationInterceptionContext(targetReference, operationModelId, parameterValues));
			
			return Json(interception.Do(() => context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues)));
		}
	}
}