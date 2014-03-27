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

		PerformInterceptionContext CreatePerformInterceptionContext(Robject target, string operationModelId, List<Rvariable> parameters);
    }
}
