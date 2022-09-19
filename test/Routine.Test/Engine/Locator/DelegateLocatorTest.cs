using Routine.Engine;

namespace Routine.Test.Engine.Locator;

public class DelegateLocatorTest
{
    static object[] Locators = new object[]
    {
        new object[] { BuildRoutine.Locator().By(ids => ids.Select(id => $"located: {id}" as object)) },
        new object[] { BuildRoutine.Locator().By((_, ids) => ids.Select(id => $"located: {id}" as object)) },
        new object[] { BuildRoutine.Locator().By(async ids => { await Task.Delay(0); return ids.Select(id => $"located: {id}" as object); }) },
        new object[] { BuildRoutine.Locator().By(async (_, ids) => { await Task.Delay(0); return ids.Select(id => $"located: {id}" as object); }) }
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
        Assert.Throws<ArgumentNullException>(() => BuildRoutine.Locator().By(null as Func<IType, List<string>, IEnumerable>));
        Assert.Throws<ArgumentNullException>(() => BuildRoutine.Locator().By(null as Func<IType, List<string>, Task<IEnumerable>>));
    }
}
