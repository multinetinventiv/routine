using Routine.Core;

namespace Routine.Client.Context;

public class DefaultClientContext : IClientContext
{
    public IObjectService ObjectService { get; }
    public Rapplication Application { get; }

    public DefaultClientContext(IObjectService objectService, Rapplication application)
    {
        ObjectService = objectService;
        Application = application;
    }
}
