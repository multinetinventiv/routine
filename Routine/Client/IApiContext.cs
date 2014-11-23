using Routine.Core;

namespace Routine.Client
{
    public interface IApiContext
    {
        IObjectService ObjectService { get; }
		Rapplication Application { get; }
    }
}
