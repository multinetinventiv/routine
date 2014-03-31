using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Service;

namespace Routine.Soa.Context
{
	public class PerformOperationInterceptionContext : OperationInterceptionContext
	{
		public PerformOperationInterceptionContext(ICoreContext coreContext, ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
			: base(coreContext, targetReference, operationModelId)
		{
			ParameterValues = parameterValues;
		}

		public List<ParameterValueData> ParameterValues { get; private set; }
	}
}
