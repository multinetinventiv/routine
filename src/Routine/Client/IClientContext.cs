using Routine.Core;

namespace Routine.Client;

public interface IClientContext
{
    IObjectService ObjectService { get; }
    Rapplication Application { get; }
}
