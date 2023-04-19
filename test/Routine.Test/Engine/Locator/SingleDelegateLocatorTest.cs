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

        Assert.That(actual[0], Is.EqualTo("located: test1"));
        Assert.That(actual[1], Is.EqualTo("located: test2"));
    }

    [Test]
    public void When_no_delegate_was_given__it_throws_ArgumentNullException()
    {
        Assert.That(() => BuildRoutine.Locator().SingleBy(null as Func<IType, string, object>), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => BuildRoutine.Locator().SingleBy(null as Func<IType, string, Task<object>>), Throws.TypeOf<ArgumentNullException>());
    }
}
