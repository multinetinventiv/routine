using Routine.Core.Service;

namespace Routine.Api
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
