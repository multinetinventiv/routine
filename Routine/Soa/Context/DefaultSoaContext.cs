using Routine.Core;
using Routine.Engine;

namespace Routine.Soa.Context
{
	public class DefaultSoaContext : ISoaContext
	{
		public ISoaConfiguration SoaConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }
		public ICoreContext CoreContext { get; private set; }

		public DefaultSoaContext(ICoreContext coreContext, ISoaConfiguration soaConfiguration, IObjectService objectService)
		{
			SoaConfiguration = soaConfiguration;
			ObjectService = objectService;
			CoreContext = coreContext;
		}
	}
}
