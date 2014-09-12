using Routine.Core.Api;

namespace Routine.Core
{
    public interface IApiContext
    {
        IObjectService ObjectService { get; }

        Rapplication Rapplication { get; }

        Robject CreateRobject();
        Rmember CreateRmember();
        Roperation CreateRoperation();
        Rparameter CreateRparameter();
        Rvariable CreateRvariable();
    }
}
