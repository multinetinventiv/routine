using System.Collections.Generic;
using Routine.Core;
using Routine.Api;

namespace Routine.Mvc.Context
{
	public class GetInterceptionContext : InterceptionContext
	{
		public GetInterceptionContext(string id, string actualModelId)
		{
			Id = id;
			ActualModelId = actualModelId;
		}

		public string Id { get; private set; }
		public string ActualModelId { get; private set; }
	}

	public class GetAsInterceptionContext : GetInterceptionContext
	{
		public GetAsInterceptionContext(string id, string actualModelId, string viewModelId)
			: base(id, actualModelId)
		{
			ViewModelId = viewModelId;
		}

		public string ViewModelId { get; private set; }
	}
}
