namespace Routine.Core.Context
{
	public class ObjectReferenceInterceptionContext : InterceptionContext
	{
		protected readonly IObjectService objectService;

		public ObjectReferenceInterceptionContext(IObjectService objectService, ObjectReferenceData targetReference)
		{
			this.objectService = objectService;

			TargetReference = targetReference;
		}

		public ObjectReferenceData TargetReference { get; private set; }

		public ObjectModel GetActualModel() { return objectService.GetObjectModel(TargetReference.ActualModelId); }
		public ObjectModel GetViewModel() { return objectService.GetObjectModel(TargetReference.ViewModelId); }
	}
}
