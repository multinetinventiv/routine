using System.Collections.Generic;
using Routine.Interception;

namespace Routine.Ui.Context
{
	public class PerformInterceptionContext : InterceptionContext
	{
		public PerformInterceptionContext(string target, ObjectViewModel objectViewModel, string operationModelId, Dictionary<string, string> parameters)
			: base(target)
		{
			ObjectViewModel = objectViewModel;
			OperationModelId = operationModelId;
			Parameters = parameters;
		}

		public ObjectViewModel ObjectViewModel
		{
			get { return this["ObjectViewModel"] as ObjectViewModel; }
			set { this["ObjectViewModel"] = value; }
		}

		public string OperationModelId
		{
			get { return this["OperationModelId"] as string; }
			set { this["OperationModelId"] = value; }
		}

		public Dictionary<string, string> Parameters
		{
			get { return this["Parameters"] as Dictionary<string, string>; }
			set { this["Parameters"] = value; }
		}
	}
}
