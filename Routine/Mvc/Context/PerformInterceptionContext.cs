using System.Collections.Generic;
using Routine.Core;
using Routine.Api;

namespace Routine.Mvc.Context
{
	public class PerformInterceptionContext : InterceptionContext
	{
		public PerformInterceptionContext(Robject target, string operationModelId, List<Rvariable> parameters)
		{
			Target = target;
			OperationModelId = operationModelId;
			Parameters = parameters;
		}

		public Robject Target { get; private set; }
		public string OperationModelId { get; private set; }
		public List<Rvariable> Parameters { get; private set; }
	}
}
