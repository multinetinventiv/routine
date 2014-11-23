using Routine.Core;

namespace Routine.Client.Context
{
    public class DefaultApiContext : IApiContext
    {
        public IObjectService ObjectService { get; private set; }
		public Rapplication Application { get; private set; }

        public DefaultApiContext(IObjectService objectService, Rapplication application)
        {
	        ObjectService = objectService;
	        Application = application;
        }
    }
}
