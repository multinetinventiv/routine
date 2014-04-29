using System.Collections.Generic;
using Routine.Api;
using Routine.Mvc.Context;

namespace Routine.Mvc
{
    public interface IMvcContext
    {
        IMvcConfiguration MvcConfiguration { get; }

        ApplicationViewModel Application { get; }

        MenuViewModel CreateMenu();
        ObjectViewModel CreateObject();
        MemberViewModel CreateMember();
        OperationViewModel CreateOperation();
        ParameterViewModel CreateParameter();
        VariableViewModel CreateVariable();

		PerformInterceptionContext CreatePerformInterceptionContext(ObjectViewModel target, string operationModelId, Dictionary<string, string> parameters);
		GetInterceptionContext CreateGetInterceptionContext(string id, string actualModelId);
		GetAsInterceptionContext CreateGetAsInterceptionContext(string id, string actualModelId, string viewModelId);

    }
}
