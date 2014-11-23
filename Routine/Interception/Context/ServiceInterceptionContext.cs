using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Interception.Context
{
	public class ServiceInterceptionContext : ObjectReferenceInterceptionContext
	{
		public ServiceInterceptionContext(
			string target, IObjectService objectService, ObjectReferenceData targetReference, 
			string operationModelId, Dictionary<string, ParameterValueData> parameterValues
		) : base(target, objectService, targetReference)
		{
			OperationModelId = operationModelId;
			ParameterValues = parameterValues;
		}

		public Dictionary<string, ParameterValueData> ParameterValues
		{
			get { return this["ParameterValues"] as Dictionary<string, ParameterValueData>; }
			set { this["ParameterValues"] = value; }
		}

		public string OperationModelId 
		{
			get { return this["OperationModelId"] as string; }
			set { this["OperationModelId"] = value; } 
		}

		public OperationModel GetOperationModel() { return GetViewModel().Operations.Single(m => m.Id == OperationModelId); }
	}
}
