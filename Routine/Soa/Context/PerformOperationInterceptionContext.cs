using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Service;

namespace Routine.Soa.Context
{
	public class PerformOperationInterceptionContext : InterceptionContext
	{
		private readonly ICoreContext coreContext;

		public PerformOperationInterceptionContext(ICoreContext coreContext, ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
		{
			this.coreContext = coreContext;

			TargetReference = targetReference;
			OperationModelId = operationModelId;
			ParameterValues = parameterValues;
		}

		public ObjectReferenceData TargetReference { get; private set; }
		public string OperationModelId { get; private set; }
		public List<ParameterValueData> ParameterValues { get; private set; }

		public DomainType TargetType { get { return coreContext.GetDomainType(TargetReference.ViewModelId); } }
		public DomainOperation TargetOperation { get { return TargetType.Operation[OperationModelId]; } }
	}
}
