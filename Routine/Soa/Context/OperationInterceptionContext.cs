using Routine.Core;

namespace Routine.Soa.Context
{
	public class OperationInterceptionContext : ObjectReferenceInterceptionContext
	{
		public OperationInterceptionContext(ICoreContext coreContext, ObjectReferenceData targetReference, string operationModelId)
			: base(coreContext, targetReference)
		{
			OperationModelId = operationModelId;
		}

		public string OperationModelId { get; private set; }

		public DomainOperation TargetOperation { get { return TargetType.Operation[OperationModelId]; } }
	}
}
