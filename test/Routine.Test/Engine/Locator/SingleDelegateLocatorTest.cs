using Routine.Engine;

namespace Routine.Test.Engine.Locator;

public class SingleDelegateLocatorTest
{
    static object[] Locators = new object[]
    {
        new object[] { BuildRoutine.Locator().SingleBy(id => $"located: {id}") },
        new object[] { BuildRoutine.Locator().SingleBy((_, id) => $"located: {id}") },
        new object[] { BuildRoutine.Locator().SingleBy(async id => { await Task.Delay(0); return $"located: {id}"; }) },
        new object[] { BuildRoutine.Locator().SingleBy(async (_, id) => { await Task.Delay(0); return $"located: {id}"; }) }
    };

    [TestCaseSource(nameof(Locators))]
    public async Task Uses_delegate_to_locate_objects(ILocator locator)
    {
        var actual = await locator.LocateAsync(type.of<string>(), new List<string> { "test1", "test2" });

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
