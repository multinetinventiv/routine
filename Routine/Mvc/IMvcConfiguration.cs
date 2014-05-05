using System;
using System.Collections.Generic;
using Routine.Api;
using Routine.Core;
using Routine.Mvc.Context;

namespace Routine.Mvc
{
	public interface IMvcConfiguration
	{
		string NullDisplayValue{get;}
		char ViewNameSeparator {get;}
		char ListValueSeparator {get;}

		IInterceptor<PerformInterceptionContext> PerformInterceptor { get; }
		IInterceptor<GetInterceptionContext> GetInterceptor { get; }
		IInterceptor<GetAsInterceptionContext> GetAsInterceptor { get; }

		IExtractor<ObjectModel, string> IndexIdExtractor{get;}
		IExtractor<ObjectModel, List<string>> MenuIdsExtractor{ get;}

		IExtractor<Rparameter, Rvariable> ParameterDefaultExtractor {get;}
		IExtractor<Rparameter, List<Robject>> ParameterOptionsExtractor{get;}

		IExtractor<ViewModelBase, string> ViewNameExtractor {get;}
		IExtractor<string, string> DisplayNameExtractor { get; }

		IExtractor<ObjectViewModel, string> ViewRouteNameExtractor{get;}
		IExtractor<ObjectViewModel, string> PerformRouteNameExtractor{get;}

		IExtractor<ObjectViewModel, Func<OperationViewModel, int>> OperationOrderExtractor{get;}
		ISelector<ObjectViewModel, Func<OperationViewModel, bool>> OperationGroupSelector{get;}

		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> MemberOrderExtractor{get;}
		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> SimpleMemberOrderExtractor{get;}
		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> TableMemberOrderExtractor{get;}

		IExtractor<OperationViewModel, bool> OperationIsAvailableExtractor{get;}
	}
}
