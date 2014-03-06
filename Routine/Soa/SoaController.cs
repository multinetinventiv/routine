using System.Collections.Generic;
using System.Web.Mvc;
using Routine.Core.Service;

namespace Routine.Soa
{
	public class SoaController : Controller
	{
		private readonly IFactory factory;

		public SoaController(IFactory factory) {
			this.factory = factory;
		}

		public JsonResult GetApplicationModel()
		{
			return Json(factory.Create<IObjectService>().GetApplicationModel(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetObjectModel(string objectModelId)
		{
			return Json(factory.Create<IObjectService>().GetObjectModel(objectModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetAvailableObjects(string objectModelId)
		{
			return Json(factory.Create<IObjectService>().GetAvailableObjects(objectModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetValue(ObjectReferenceData reference)
		{
			return Json(factory.Create<IObjectService>().GetValue(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Get(ObjectReferenceData reference)
		{
			return Json(factory.Create<IObjectService>().Get(reference), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMember(ObjectReferenceData reference, string memberModelId)
		{
			return Json(factory.Create<IObjectService>().GetMember(reference, memberModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetOperation(ObjectReferenceData reference, string operationModelId)
		{
			return Json(factory.Create<IObjectService>().GetMember(reference, operationModelId), JsonRequestBehavior.AllowGet);
		}

		public JsonResult PerformOperation(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
		{
			return Json(factory.Create<IObjectService>().PerformOperation(targetReference, operationModelId, parameterValues));
		}
	}
}