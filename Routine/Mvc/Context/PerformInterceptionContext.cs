using System.Collections.Generic;
using Routine.Core;
using Routine.Api;

namespace Routine.Mvc.Context
{
	public class PerformInterceptionContext : InterceptionContext
	{
		public PerformInterceptionContext(ObjectViewModel target, string operationModelId, Dictionary<string, string> parameters)
		{
			Target = target;
			OperationModelId = operationModelId;
			Parameters = parameters;
		}

		public ObjectViewModel Target { get; private set; }
		public string OperationModelId { get; private set; }
		public Dictionary<string, string> Parameters { get; private set; }
	}
}
