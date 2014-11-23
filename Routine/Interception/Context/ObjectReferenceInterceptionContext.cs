using Routine.Core;

namespace Routine.Interception.Context
{
	public class ObjectReferenceInterceptionContext : InterceptionContext
	{
		protected readonly IObjectService objectService;

		public ObjectReferenceInterceptionContext(string target, IObjectService objectService, ObjectReferenceData targetReference)
			: base(target)
		{
			this.objectService = objectService;

			TargetReference = targetReference;
		}

		public ObjectReferenceData TargetReference
		{
			get { return this["TargetReference"] as ObjectReferenceData; }
			set { this["TargetReference"] = value; }
		}

		public ObjectModel GetActualModel() { return objectService.GetObjectModel(TargetReference.ActualModelId); }
		public ObjectModel GetViewModel() { return objectService.GetObjectModel(TargetReference.ViewModelId); }
	}
}
