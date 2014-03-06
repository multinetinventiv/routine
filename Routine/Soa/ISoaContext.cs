using Routine.Core.Service;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }
	}
}
