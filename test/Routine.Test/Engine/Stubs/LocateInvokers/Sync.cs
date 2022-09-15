using Routine.Engine;

namespace Routine.Test.Engine.Stubs.LocateInvokers;

public class Sync : ILocateInvoker
{
    public List<object> InvokeLocate(ILocator testing, IType type, List<string> ids)
        => testing.Locate(type, ids);
}
