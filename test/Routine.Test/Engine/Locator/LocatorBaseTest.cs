using Routine.Engine;
using Routine.Engine.Locator;

namespace Routine.Test.Engine.Locator;

public class LocatorBaseTest
{
    #region SetUp & Helpers

    public class TestLocator : LocatorBase<TestLocator>
    {
        private readonly bool provideDifferentNumberOfObjects;

        public TestLocator(bool provideDifferentNumberOfObjects)
        {
            this.provideDifferentNumberOfObjects = provideDifferentNumberOfObjects;
        }

        protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
        {
            var result = ids.Select(_ => (object)null).ToList();

            if (provideDifferentNumberOfObjects)
            {
                result.Add(null);
            }

            return Task.FromResult(result);
        }
    }

    #endregion

    [Test]
    public async Task Locate_throws_cannot_locate_exception_when_result_is_null_and_locator_does_not_accept_null()
    {
        var testing = new TestLocator(false);
        var testingInterface = (ILocator)testing;

        testing.AcceptNullResult(true);

        var actual = await testingInterface.LocateAsync(type.of<string>(), new List<string> { "dummy" });
        Assert.IsNull(actual[0]);

        testing.AcceptNullResult(false);

        Assert.ThrowsAsync<CannotLocateException>(async () => await testingInterface.LocateAsync(type.of<string>(), new List<string> { "dummy" }));
    }

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_count_is_different_than_given_id_count()
    {
        var testing = new TestLocator(true);
        var testingInterface = (ILocator)testing;

        Assert.ThrowsAsync<CannotLocateException>(async () => await testingInterface.LocateAsync(type.of<string>(), new List<string> { "dummy" }));
    }
}
