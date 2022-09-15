using Routine.Engine.Locator;
using Routine.Test.Core;
using Routine.Test.Engine.Stubs.LocateInvokers;

namespace Routine.Test.Engine.Locator;

[TestFixture(typeof(Async))]
[TestFixture(typeof(Sync))]
public class DelegateLocatorTest<TLocateInvoker> : CoreTestBase
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
    public void Uses_delegate_to_locate_an_object()
    {
        var testing = new DelegateLocator((_, ids) => ids.Select(id => "located: " + id).Cast<object>().ToList());

        var actual = invoker.InvokeLocate(testing, type.of<string>(), new List<string> { "test1", "test2" });

        Assert.AreEqual("located: test1", actual[0]);
        Assert.AreEqual("located: test2", actual[1]);
    }

    [Test]
    public void When_no_delegate_was_given_throws_ArgumentNullException()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<ArgumentNullException>(() => new DelegateLocator(null));
    }

    [Test]
    public void Facade_tests()
    {
        Assert.Fail("not tested");
    }
}
