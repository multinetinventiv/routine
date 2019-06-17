namespace Routine.Ui.Context
{
	public class GetAsInterceptionContext : GetInterceptionContext
	{
		public GetAsInterceptionContext(string target, string id, string actualModelId, string viewModelId)
			: base(target, id, actualModelId)
		{
			ViewModelId = viewModelId;
		}

		public string ViewModelId
		{
			get { return this["ViewModelId"] as string; }
			set { this["ViewModelId"] = value; }
		}
	}
}