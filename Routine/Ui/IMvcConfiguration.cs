using System;
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

		Rvariable GetDefault(ParameterViewModel parameterViewModel);
		
		//TODO List<Rvariable> donmeli
		List<Robject> GetOptions(ParameterViewModel parameterViewModel);
		
		//TODO operation donmeli
		Robject GetSearcher(Rparameter parameter);

		string GetViewName(ViewModelBase viewModel);
		string GetDisplayName(string key);

		string GetViewRouteName(ObjectViewModel objectViewModel);
		string GetPerformRouteName(ObjectViewModel objectViewModel);
		bool GetHasDetail(ObjectViewModel objectViewModel);

		int GetOrder(OperationViewModel operationViewModel, OperationTypes operationTypes);
		int GetOrder(OptionViewModel optionViewModel);
		int GetOrder(MemberViewModel memberViewModel, MemberTypes memberTypes);

		bool IsAvailable(OperationViewModel operationViewModel);
		bool IsRendered(OperationViewModel operationViewModel);
		OperationTypes GetOperationTypes(OperationViewModel operationViewModel);

		bool IsRendered(MemberViewModel memberViewModel);
		MemberTypes GetMemberTypes(MemberViewModel memberViewModel);
	}

	[Flags]
	public enum OperationTypes
	{
		None = 0,
		Page = 1,
		Table = 2,
		Search = 4,
	}

	[Flags]
	public enum MemberTypes
	{
		None = 0,
		PageNameValue = 1,
		PageTable = 2,
		TableColumn = 4,
	}

	public enum InterceptionTarget
	{
		Perform,
		Get,
		GetAs,
		Index,
	}
}
