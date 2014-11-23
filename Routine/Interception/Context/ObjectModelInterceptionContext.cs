using Routine.Core;

namespace Routine.Interception.Context
{
	public class ObjectModelInterceptionContext : InterceptionContext
	{
		protected readonly IObjectService objectService;

		public ObjectModelInterceptionContext(string target, IObjectService objectService, string objectModelId) 
			: base(target)
		{
			this.objectService = objectService;

			ObjectModelId = objectModelId;
		}

		public string ObjectModelId
		{
			get { return this["ObjectModelId"] as string; }
			set { this["ObjectModelId"] = value; }
		}
	}
}
