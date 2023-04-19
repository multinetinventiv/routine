using Routine.Core.Reflection;

namespace Routine.Test.Core.Reflection;

[TestFixture]
public class ReflectionMethodInvokerSyncTest : ReflectionMethodInvokerContract
{
    protected override object Invoke(IMethodInvoker invoker, object target, params object[] args) => invoker.Invoke(target, args);

    private string _testTaskAsyncResponse;
    public async Task TestVoidAsync(TimeSpan delay, string response) { await Task.Delay(delay); _testTaskAsyncResponse = response; }

    [Test]
    public void Given_an_async_method__it_waits_for_returned_task()
    {
        var expected = $"{Guid.NewGuid()}";

        var testing = InvokerFor(nameof(TestVoidAsync));

        testing.Invoke(this, TimeSpan.FromMilliseconds(10), expected);

        Assert.That(_testTaskAsyncResponse, Is.EqualTo(expected));
    }

    public async Task<string> TestAsync(TimeSpan delay, string response) { await Task.Delay(delay); return response; }

    [Test]
    public void Given_an_async_method_with_a_result__it_waits_and_returns_the_result()
    {
        var testing = InvokerFor(nameof(TestAsync));

        var actual = testing.Invoke(this, TimeSpan.FromMilliseconds(10), "test");

        Assert.That(actual, Is.EqualTo("test"));
    }
}
