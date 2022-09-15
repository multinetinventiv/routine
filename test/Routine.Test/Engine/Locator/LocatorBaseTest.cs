using Routine.Engine;
using Routine.Test.Core;
using Routine.Test.Engine.Stubs.LocateInvokers;
using Routine.Test.Engine.Stubs.Locators;

namespace Routine.Test.Engine.Locator;

[TestFixture(typeof(Async))]
[TestFixture(typeof(Sync))]
public class LocatorBaseTest<TLocateInvoker> : CoreTestBase
    where TLocateInvoker : ILocateInvoker, new()
{
    #region SetUp

    private TLocateInvoker invoker;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        invoker = new TLocateInvoker();
    }

    #endregion

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_is_null_and_locator_does_not_accept_null()
    {
        var testing = new Base(false);

        testing.AcceptNullResult(true);

        var actual = invoker.InvokeLocate(testing, type.of<string>(), new List<string> { "dummy" });
        Assert.IsNull(actual[0]);

        testing.AcceptNullResult(false);

        Assert.Throws<CannotLocateException>(() => invoker.InvokeLocate(testing, type.of<string>(), new List<string> { "dummy" }));
    }

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_count_is_different_than_given_id_count()
    {
        var testing = new Base(true);

        Assert.Throws<CannotLocateException>(() => invoker.InvokeLocate(testing, type.of<string>(), new List<string> { "dummy" }));
    }
}
