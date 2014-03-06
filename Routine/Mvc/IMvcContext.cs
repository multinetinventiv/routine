using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
