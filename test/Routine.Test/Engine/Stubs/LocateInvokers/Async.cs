using Routine.Core.Runtime;
using Routine.Engine;

namespace Routine.Test.Engine.Stubs.LocateInvokers;

public class Async : ILocateInvoker
{
    public List<object> InvokeLocate(ILocator testing, IType type, List<string> ids)
        => testing.LocateAsync(type, ids).WaitAndGetResult();
}
