using Routine.Test.Engine.Stubs.LocateInvokers;

namespace Routine.Test.Engine.Locator;

[TestFixture(typeof(Async))]
[TestFixture(typeof(Sync))]
public class ConstantLocator<TLocateInvoker> : LocatorTestBase<TLocateInvoker>
    where TLocateInvoker : ILocateInvoker, new()
{
    [Test]
    public void Always_locates_given_constant()
    {
        var locator = BuildRoutine.Locator().Constant("constant");

        var actual = invoker.InvokeLocate(locator, type.of<string>(), new List<string> { string.Empty, string.Empty });

        Assert.AreEqual("constant", actual[0]);
        Assert.AreEqual("constant", actual[1]);
    }
}
