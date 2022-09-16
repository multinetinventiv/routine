using Routine.Engine;
using Routine.Test.Engine.Stubs.LocateInvokers;

namespace Routine.Test.Engine.Locator;

[TestFixture(typeof(Async))]
[TestFixture(typeof(Sync))]
public class SingleDelegateLocatorTest<TLocateInvoker> : LocatorTestBase<TLocateInvoker>
    where TLocateInvoker : ILocateInvoker, new()
{
    static object[] Locators = new object[]
    {
        new object[] { BuildRoutine.Locator().SingleBy(id => $"located: {id}") },
        new object[] { BuildRoutine.Locator().SingleBy((_, id) => $"located: {id}") },
        new object[] { BuildRoutine.Locator().SingleBy(async id => { await Task.Delay(0); return $"located: {id}"; }) },
        new object[] { BuildRoutine.Locator().SingleBy(async (_, id) => { await Task.Delay(0); return $"located: {id}"; }) }
    };

    [TestCaseSource(nameof(Locators))]
    public void Uses_delegate_to_locate_objects(ILocator locator)
    {
        var actual = invoker.InvokeLocate(locator, type.of<string>(), new List<string> { "test1", "test2" });

        Assert.AreEqual("located: test1", actual[0]);
        Assert.AreEqual("located: test2", actual[1]);
    }

    [Test]
    public void When_no_delegate_was_given__it_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BuildRoutine.Locator().SingleBy(null as Func<IType, string, object>));
        Assert.Throws<ArgumentNullException>(() => BuildRoutine.Locator().SingleBy(null as Func<IType, string, Task<object>>));
    }
}
