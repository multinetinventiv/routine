using Routine.Engine;

namespace Routine.Test.Engine.Stubs.LocateInvokers;

public interface ILocateInvoker
{
    List<object> InvokeLocate(ILocator testing, IType type, List<string> ids);
}
