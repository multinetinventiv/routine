using Routine.Core.Reflection;
using Routine.Core.Runtime;

namespace Routine.Test.Core.Reflection;

[TestFixture]
public class ReflectionMethodInvokerAsyncTest : ReflectionMethodInvokerContract
{
    protected override object Invoke(IMethodInvoker invoker, object target, params object[] args) => invoker.InvokeAsync(target, args).WaitAndGetResult();

    public async Task<string> TestAsync(TimeSpan delay, string response) { await Task.Delay(delay); return response; }

    [Test]
    public async Task Given_a_sync_method__it_directly_returns_the_result()
    {
        var testing = InvokerFor(nameof(Test));

        var actual = await testing.InvokeAsync(this, "test");

        Assert.AreEqual("test", actual);
    }

    [Test]
    public async Task Given_an_async_method__it_wraps_returned_task_to_return_object()
    {
        var testing = InvokerFor(nameof(TestAsync));

        var actual = await testing.InvokeAsync(this, TimeSpan.FromMilliseconds(10), "test");

        Assert.AreEqual("test", actual);
    }

    [Test]
    public async Task Retesting_ThrowAsync_case_in_an_async_method__because_base_contract_tests_it_in_a_sync_method()
    {
        var expected = new CustomException("message");

        var testing = InvokerFor(nameof(ThrowAsync));

        try
        {
            await testing.InvokeAsync(this, expected);
            Assert.Fail("exception not thrown");
        }
        catch (Exception actual)
        {
            Assert.AreSame(expected, actual);
        }
    }
}
