using Routine.Core;

namespace Routine.Client.Context
{
    public class DefaultClientContext : IClientContext
    {
        public IObjectService ObjectService { get; private set; }
		public Rapplication Application { get; private set; }

        public DefaultClientContext(IObjectService objectService, Rapplication application)
        {
	        ObjectService = objectService;
	        Application = application;
        }
    }
}
