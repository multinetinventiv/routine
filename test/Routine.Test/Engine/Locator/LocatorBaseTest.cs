using Routine.Engine;
using Routine.Engine.Locator;

namespace Routine.Test.Engine.Locator;

public class LocatorBaseTest
{
    #region SetUp & Helpers

    private class TestLocator : LocatorBase<TestLocator>
    {
        private readonly bool _provideDifferentNumberOfObjects;

        public TestLocator(bool provideDifferentNumberOfObjects)
        {
            _provideDifferentNumberOfObjects = provideDifferentNumberOfObjects;
        }

        protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
        {
            var result = ids.Select(_ => (object)null).ToList();

            if (_provideDifferentNumberOfObjects)
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
        var testingInterface = testing as ILocator;

        testing.AcceptNullResult(true);

        var actual = await testingInterface.LocateAsync(type.of<string>(), new List<string> { "dummy" });
        Assert.That(actual[0], Is.Null);

        testing.AcceptNullResult(false);

        Assert.That(async () => await testingInterface.LocateAsync(type.of<string>(), new List<string> { "dummy" }), Throws.TypeOf<CannotLocateException>());
    }

    [Test]
    public void Locate_throws_cannot_locate_exception_when_result_count_is_different_than_given_id_count()
    {
        var testing = new TestLocator(true);
        var testingInterface = testing as ILocator;

        Assert.That(async () => await testingInterface.LocateAsync(type.of<string>(), new List<string> { "dummy" }), Throws.TypeOf<CannotLocateException>());
    }
}
