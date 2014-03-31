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
			var interception = context.SoaConfiguration.GetApplicationModelInterceptor.Intercept(context.CreateInterceptionContext());

			return Json(interception.Do(() => context.ObjectService.GetApplicationModel()), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetObjectModel(string objectModelId)
		{
			var interception = context.SoaConfiguration.GetObjectModelInterceptor.Intercept(context.CreateObjectModelInterceptionContext(objectModelId));

			return Json(interception.Do(() => context.ObjectService.GetObjectModel(objectModelId)), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetAvailableObjects(string objectModelId)
		{
			var interception = context.SoaConfiguration.GetAvailableObjectsInterceptor.Intercept(context.CreateObjectModelInterceptionContext(objectModelId));

			return Json(interception.Do(() => context.ObjectService.GetAvailableObjects(objectModelId)), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			var interception = context.SoaConfiguration.GetValueInterceptor.Intercept(context.CreateObjectReferenceInterceptionContext(reference));

			return Json(interception.Do(() => context.ObjectService.GetValue(reference)), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			var interception = context.SoaConfiguration.GetInterceptor.Intercept(context.CreateObjectReferenceInterceptionContext(reference));

			return Json(interception.Do(() => context.ObjectService.Get(reference)), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMember(ObjectReferenceData reference, string memberModelId)
		{
			var interception = context.SoaConfiguration.GetMemberInterceptor.Intercept(context.CreateMemberInterceptionContext(reference, memberModelId));

			return Json(interception.Do(() => context.ObjectService.GetMember(reference, memberModelId)), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetOperation(ObjectReferenceData reference, string operationModelId)
		{
			var interception = context.SoaConfiguration.GetOperationInterceptor.Intercept(context.CreateOperationInterceptionContext(reference, operationModelId));

			return Json(interception.Do(() => context.ObjectService.GetMember(reference, operationModelId)), JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
		{
			var interception = context.SoaConfiguration.PerformOperationInterceptor.Intercept(context.CreatePerformOperationInterceptionContext(targetReference, operationModelId, parameterValues));
			
			return Json(interception.Do(() => context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues)));
		}
	}
}