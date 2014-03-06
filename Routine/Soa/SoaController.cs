using System.Collections.Generic;
using System.Web.Mvc;
using Routine.Core.Service;

namespace Routine.Soa
{
	public class SoaController : Controller
	{
        private readonly ISoaContext context;

		public SoaController(ISoaContext context)
        {
            this.context = context;
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
			return Json(context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues));
		}
	}
}