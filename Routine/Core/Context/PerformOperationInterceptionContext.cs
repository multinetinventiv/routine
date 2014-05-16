using System.Collections.Generic;

namespace Routine.Core.Context
{
	public class PerformOperationInterceptionContext : OperationInterceptionContext
	{
		public PerformOperationInterceptionContext(IObjectService objectService, ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues)
			: base(objectService, targetReference, operationModelId)
		{
			ParameterValues = parameterValues;
		}

		public Dictionary<string, ReferenceData> ParameterValues { get; private set; }
	}
}
