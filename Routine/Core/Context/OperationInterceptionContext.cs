using System.Linq;

namespace Routine.Core.Context
{
	public class OperationInterceptionContext : ObjectReferenceInterceptionContext
	{
		public OperationInterceptionContext(IObjectService objectService, ObjectReferenceData targetReference, string operationModelId)
			: base(objectService, targetReference)
		{
			OperationModelId = operationModelId;
		}

		public string OperationModelId { get; private set; }

		public OperationModel GetOperationModel() { return GetViewModel().Operations.Single(m => m.Id == OperationModelId); }
	}
}
