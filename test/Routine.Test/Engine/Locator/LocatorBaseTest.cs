using Routine.Engine;
using Routine.Test.Engine.Stubs.Locators;

namespace Routine.Test.Engine.Locator;

public class LocatorBaseTest
{
    [Test]
    public async Task Locate_throws_cannot_locate_exception_when_result_is_null_and_locator_does_not_accept_null()
    {
        var locatorBase = new Base(false);
        var locator = locatorBase as ILocator;

        locatorBase.AcceptNullResult(true);

        var actual = await locator.LocateAsync(type.of<string>(), new List<string> { "dummy" });
        Assert.IsNull(actual[0]);

        locatorBase.AcceptNullResult(false);

        Assert.ThrowsAsync<CannotLocateException>(async () => await locator.LocateAsync(type.of<string>(), new List<string> { "dummy" }));
    }

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_count_is_different_than_given_id_count()
    {
        var locator = new Base(true) as ILocator;

        Assert.ThrowsAsync<CannotLocateException>(async () => await locator.LocateAsync(type.of<string>(), new List<string> { "dummy" }));
    }
}
