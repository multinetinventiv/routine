using System.Collections.Generic;
using Routine.Core;

namespace Routine.Interception.Context
{
	public class ServiceInterceptionContext : ObjectReferenceInterceptionContext
	{
		public ServiceInterceptionContext(
			string target, IObjectService objectService, ReferenceData targetReference,
			string operationName, Dictionary<string, ParameterValueData> parameterValues
		)
			: base(target, objectService, targetReference)
		{
			OperationName = operationName;
			ParameterValues = parameterValues;
		}

		public Dictionary<string, ParameterValueData> ParameterValues
		{
			get { return this["ParameterValues"] as Dictionary<string, ParameterValueData>; }
			set { this["ParameterValues"] = value; }
		}

		public string OperationName
		{
			get { return this["OperationName"] as string; }
			set { this["OperationName"] = value; }
		}

		public OperationModel OperationModel { get { return ViewModel.Operation[OperationName]; } }
	}
}
