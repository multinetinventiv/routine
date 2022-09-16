using Routine.Engine;
using Routine.Test.Engine.Stubs.LocateInvokers;

namespace Routine.Test.Engine.Locator;

[TestFixture(typeof(Async))]
[TestFixture(typeof(Sync))]
public class SingletonLocatorTest<TLocateInvoker> : LocatorTestBase<TLocateInvoker>
    where TLocateInvoker : ILocateInvoker, new()
{
    static object[] Locators = new object[]
    {
        new object[] { BuildRoutine.Locator().Singleton(t => $"located: {t}") },
        new object[] { BuildRoutine.Locator().Singleton(async t => { await Task.Delay(0); return $"located: {t}"; }) }
    };

    [TestCaseSource(nameof(Locators))]
    public void Uses_delegate_to_locate_objects(ILocator locator)
    {
        var actual = invoker.InvokeLocate(locator, type.of<string>(), new List<string> { string.Empty, string.Empty });

        Assert.AreEqual("located: System.String", actual[0]);
        Assert.AreEqual("located: System.String", actual[1]);
    }

    [Test]
    public void When_no_delegate_was_given__it_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BuildRoutine.Locator().Singleton(null as Func<IType, IEnumerable>));
        Assert.Throws<ArgumentNullException>(() => BuildRoutine.Locator().Singleton(null as Func<IType, Task<IEnumerable>>));
    }
}
