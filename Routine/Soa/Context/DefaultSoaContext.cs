using Routine.Core.Service;

namespace Routine.Soa.Context
{
	public class DefaultSoaContext : ISoaContext
	{
		public ISoaConfiguration SoaConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }

		public DefaultSoaContext(ISoaConfiguration soaConfiguration, IObjectService objectService)
		{
			SoaConfiguration = soaConfiguration;
			ObjectService = objectService;
		}
	}
}
