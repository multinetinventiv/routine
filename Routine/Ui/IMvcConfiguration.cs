using System.Collections.Generic;
using Routine.Client;
using Routine.Interception;

namespace Routine.Ui
{
	public interface IMvcConfiguration
	{
		string GetNullDisplayValue();
		char GetViewNameSeparator();
		char GetListValueSeparator();

		IInterceptor<InterceptionContext> GetInterceptor(InterceptionTarget target);

		string GetIndexId(Rtype type);
		List<string> GetMenuIds(Rtype type);

		Rvariable GetDefault(Rparameter parameter);
		List<Robject> GetOptions(Rparameter parameter);
		Robject GetSearcher(Rparameter parameter);

		string GetViewName(ViewModelBase viewModel);
		string GetDisplayName(string key);

		string GetViewRouteName(ObjectViewModel objectViewModel);
		string GetPerformRouteName(ObjectViewModel objectViewModel);

		int GetOperationOrder(OperationViewModel operationViewModel);
		int GetMemberOrder(MemberViewModel memberViewModel);
		int GetSimpleMemberOrder(MemberViewModel memberViewModel);
		int GetTableMemberOrder(MemberViewModel memberViewModel);

		bool IsAvailable(OperationViewModel operationViewModel);
		bool IsRendered(OperationViewModel operationViewModel);
		bool IsSimple(OperationViewModel operationViewModel);

		bool IsRendered(MemberViewModel memberViewModel);
		bool IsSimple(MemberViewModel memberViewModel);
		bool IsTable(MemberViewModel memberViewModel);
	}

	public enum InterceptionTarget
	{
		Perform,
		Get,
		GetAs,
	}
}
