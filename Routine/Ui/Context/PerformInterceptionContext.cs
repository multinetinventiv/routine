using System.Collections.Generic;
using Routine.Interception;

namespace Routine.Ui.Context
{
	public class PerformInterceptionContext : InterceptionContext
	{
		public PerformInterceptionContext(string target, ObjectViewModel objectViewModel, string operationName, Dictionary<string, string> parameters)
			: base(target)
		{
			ObjectViewModel = objectViewModel;
			OperationName = operationName;
			Parameters = parameters;
		}

		public ObjectViewModel ObjectViewModel
		{
			get { return this["ObjectViewModel"] as ObjectViewModel; }
			set { this["ObjectViewModel"] = value; }
		}

		public string OperationName
		{
			get { return this["OperationName"] as string; }
			set { this["OperationName"] = value; }
		}

		public Dictionary<string, string> Parameters
		{
			get { return this["Parameters"] as Dictionary<string, string>; }
			set { this["Parameters"] = value; }
		}
	}
}
