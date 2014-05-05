using System.Collections.Generic;
using Routine.Core;

namespace Routine.Soa.Context
{
	public class PerformOperationInterceptionContext : OperationInterceptionContext
	{
		public PerformOperationInterceptionContext(ICoreContext coreContext, ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues)
			: base(coreContext, targetReference, operationModelId)
		{
			ParameterValues = parameterValues;
		}

		public Dictionary<string, ReferenceData> ParameterValues { get; private set; }
	}
}
