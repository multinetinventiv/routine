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
			return Json(
				context.SoaConfiguration.GetApplicationModelInterceptor.Intercept(
					context.CreateInterceptionContext(), 
					() => context.ObjectService.GetApplicationModel()), 
				JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetObjectModel(string objectModelId)
		{
			return Json(
				context.SoaConfiguration.GetObjectModelInterceptor.Intercept(
					context.CreateObjectModelInterceptionContext(objectModelId), 
					() => context.ObjectService.GetObjectModel(objectModelId)), 
				JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetAvailableObjects(string objectModelId)
		{
			return Json(
				context.SoaConfiguration.GetAvailableObjectsInterceptor.Intercept(
					context.CreateObjectModelInterceptionContext(objectModelId), 
					() => context.ObjectService.GetAvailableObjects(objectModelId)), 
				JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			return Json(
				context.SoaConfiguration.GetValueInterceptor.Intercept(
					context.CreateObjectReferenceInterceptionContext(reference), 
					() => context.ObjectService.GetValue(reference)), 
				JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			return Json(
				context.SoaConfiguration.GetInterceptor.Intercept(
					context.CreateObjectReferenceInterceptionContext(reference),
					() => context.ObjectService.Get(reference))
				, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMember(ObjectReferenceData reference, string memberModelId)
		{
			return Json(
				context.SoaConfiguration.GetMemberInterceptor.Intercept(
					context.CreateMemberInterceptionContext(reference, memberModelId), 
					() => context.ObjectService.GetMember(reference, memberModelId)), 
				JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetOperation(ObjectReferenceData reference, string operationModelId)
		{
			return Json(
				context.SoaConfiguration.GetOperationInterceptor.Intercept(
					context.CreateOperationInterceptionContext(reference, operationModelId), 
					() => context.ObjectService.GetMember(reference, operationModelId)), 
				JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
		{
			return Json(
				context.SoaConfiguration.PerformOperationInterceptor.Intercept(
					context.CreatePerformOperationInterceptionContext(targetReference, operationModelId, parameterValues),
					() => context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues)));
		}
	}
}