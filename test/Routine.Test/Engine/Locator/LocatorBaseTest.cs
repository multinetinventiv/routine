using Routine.Engine.Locator;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Locator;

[TestFixture]
public class LocatorBaseTest : CoreTestBase
{
    #region SetUp & Helpers

    private class TestLocator : LocatorBase<TestLocator>
    {
        private readonly bool provideDifferentNumberOfObjects;

        public TestLocator(bool provideDifferentNumberOfObjects)
        {
            this.provideDifferentNumberOfObjects = provideDifferentNumberOfObjects;
        }

        protected override List<object> Locate(IType type, List<string> ids)
        {
            var result = ids.Select(_ => (object)null).ToList();

            if (provideDifferentNumberOfObjects)
            {
                result.Add(null);
            }

            return result;
        }
    }

    #endregion

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_is_null_and_locator_does_not_accept_null()
    {
        var testing = new TestLocator(false);
        var testingInterface = (ILocator)testing;

        testing.AcceptNullResult(true);

        var actual = testingInterface.Locate(type.of<string>(), new List<string> { "dummy" });
        Assert.IsNull(actual[0]);

        testing.AcceptNullResult(false);

        Assert.Throws<CannotLocateException>(() => testingInterface.Locate(type.of<string>(), new List<string> { "dummy" }));
    }

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_count_is_different_than_given_id_count()
    {
        var testing = new TestLocator(true);
        var testingInterface = testing as ILocator;

        Assert.Throws<CannotLocateException>(() => testingInterface.Locate(type.of<string>(), new List<string> { "dummy" }));
    }
}
