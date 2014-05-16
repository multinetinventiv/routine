namespace Routine.Core.Context
{
	public class ObjectModelInterceptionContext : InterceptionContext
	{
		protected readonly IObjectService objectService;

		public ObjectModelInterceptionContext(IObjectService objectService, string objectModelId)
		{
			this.objectService = objectService;

			ObjectModelId = objectModelId;
		}

		public string ObjectModelId{ get; private set; }

		public ObjectModel GetObjectModel() { return objectService.GetObjectModel(ObjectModelId); }
	}
}
