using Routine.Interception;

namespace Routine.Ui.Context
{
	public class GetInterceptionContext : InterceptionContext
	{
		public GetInterceptionContext(string target, string id, string actualModelId)
			: base(target)
		{
			Id = id;
			ActualModelId = actualModelId;
		}

		public string Id
		{
			get { return this["Id"] as string; }
			set { this["Id"] = value; }
		}

		public string ActualModelId
		{
			get { return this["ActualModelId"] as string; }
			set { this["ActualModelId"] = value; }
		}
	}
}
