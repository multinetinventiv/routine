using Routine.Core;

namespace Routine.Api.Context
{
    public class DefaultApiContext : IApiContext
    {
        public IObjectService ObjectService { get; private set; }

        public DefaultApiContext(IObjectService objectService)
        {
            ObjectService = objectService;
        }

        public Rapplication Rapplication { get; set; }

        public Robject CreateRobject()
        {
            return new Robject(this);
        }

        public Rmember CreateRmember()
        {
            return new Rmember(this);
        }

        public Roperation CreateRoperation()
        {
            return new Roperation(this);
        }

        public Rparameter CreateRparameter()
        {
            return new Rparameter(this);
        }

        public Rvariable CreateRvariable()
        {
            return new Rvariable(this);
        }
    }
}
