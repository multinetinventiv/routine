using Routine.Engine;

namespace Routine.Test.Engine.Locator;

public class SingletonLocatorTest
{
    static object[] Locators = new object[]
    {
        new object[] { BuildRoutine.Locator().Singleton(t => $"located: {t}") },
        new object[] { BuildRoutine.Locator().Singleton(async t => { await Task.Delay(0); return $"located: {t}"; }) }
    };

    [TestCaseSource(nameof(Locators))]
    public async Task Uses_delegate_to_locate_objects(ILocator locator)
    {
        var actual = await locator.LocateAsync(type.of<string>(), new List<string> { string.Empty, string.Empty });

        Assert.That(actual[0], Is.EqualTo("located: System.String"));
        Assert.That(actual[1], Is.EqualTo("located: System.String"));
    }

    [Test]
    public void When_no_delegate_was_given__it_throws_ArgumentNullException()
    {
        Assert.That(() => BuildRoutine.Locator().Singleton(null as Func<IType, IEnumerable>), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => BuildRoutine.Locator().Singleton(null as Func<IType, Task<IEnumerable>>), Throws.TypeOf<ArgumentNullException>());
    }
}
