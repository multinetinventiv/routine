using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Routine.Client;
using Routine.Interception;

namespace Routine.Ui
{
	public interface IMvcConfiguration
	{
		string GetNullDisplayValue();
		char GetViewNameSeparator();
		char GetListValueSeparator();
		string GetDefaultObjectId();
		string GetRootPath();
		Assembly GetThemeAssembly();
		string GetThemeNamespace();
		List<Assembly> GetUiAssemblies();

		Action<HttpCachePolicy> GetCachePolicyAction(string virtualPath);
		IInterceptor<InterceptionContext> GetInterceptor(InterceptionTarget target);

		string GetIndexId(Rtype type);
		List<string> GetMenuIds(Rtype type);

		Rvariable GetDefault(ParameterViewModel parameterViewModel);
		
		//TODO should return List<Rvariable> 
		List<Robject> GetOptions(ParameterViewModel parameterViewModel);
		
		//TODO should return List<Roperation>
		Robject GetSearcher(Rparameter parameter);

		string GetViewName(ViewModelBase viewModel);
		string GetDisplayName(string key);

		bool GetHasDetail(ObjectViewModel objectViewModel);

		int GetOrder(OperationViewModel operationViewModel, OperationTypes operationTypes);
		int GetOrder(OptionViewModel optionViewModel);
		int GetOrder(MemberViewModel memberViewModel, MemberTypes memberTypes);

		bool IsAvailable(OperationViewModel operationViewModel);
		bool IsRendered(OperationViewModel operationViewModel);
		OperationTypes GetOperationTypes(OperationViewModel operationViewModel);
		bool GetConfirmationRequired(OperationViewModel operationViewModel);

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
