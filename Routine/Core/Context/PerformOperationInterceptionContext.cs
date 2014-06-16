using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Context
{
	public class PerformOperationInterceptionContext : ObjectReferenceInterceptionContext
	{
		public PerformOperationInterceptionContext(IObjectService objectService, ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
			: base(objectService, targetReference)
		{
			OperationModelId = operationModelId;
			ParameterValues = parameterValues;
		}

		public Dictionary<string, ParameterValueData> ParameterValues { get; private set; }

		public string OperationModelId { get; private set; }

		public OperationModel GetOperationModel() { return GetViewModel().Operations.Single(m => m.Id == OperationModelId); }
	}
}
