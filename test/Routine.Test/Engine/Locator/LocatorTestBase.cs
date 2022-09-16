using Routine.Test.Core;
using Routine.Test.Engine.Stubs.LocateInvokers;

namespace Routine.Test.Engine.Locator;

public abstract class LocatorTestBase<TLocateInvoker> : CoreTestBase
    where TLocateInvoker : ILocateInvoker, new()
{
    protected TLocateInvoker invoker;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        invoker = new TLocateInvoker();
    }
}
