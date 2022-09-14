using Routine.Core.Reflection;
using Routine.Core.Runtime;

namespace Routine.Test.Core.Reflection;

[TestFixture]
public class ReflectionOptimizerAsyncTest : ReflectionOptimizerContract
{
    protected override object Invoke(IMethodInvoker invoker, object target, params object[] args) => invoker.InvokeAsync(target, args).WaitAndGetResult();

    [Test]
    public async Task Returns_result_of_sync_methods_without_doing_anything()
    {
        mock.Setup(o => o.StringMethod()).Returns("test");

        var testing = InvokerFor<OptimizedClass>(nameof(OptimizedClass.StringMethod));

        var actual = await testing.InvokeAsync(target);

        Assert.AreEqual("test", actual);
    }

    [Test]
    public async Task Awaits_async_methods()
    {
        var testing = InvokerFor<OptimizedClass>(nameof(OptimizedClass.AsyncVoidMethod));

        var actual = await testing.InvokeAsync(target);

        Assert.IsNull(actual);
        mock.Verify(o => o.AsyncVoidMethod());
    }

    [Test]
    public async Task Wraps_and_returns_result_of_the_task_returned()
    {
        mock.Setup(o => o.AsyncStringMethod()).ReturnsAsync("test");

        var testing = InvokerFor<OptimizedClass>(nameof(OptimizedClass.AsyncStringMethod));

        var actual = await testing.InvokeAsync(target);

        Assert.AreEqual("test", actual);
    }

    [TestCase(nameof(OptimizedClass.VoidMethod))]
    [TestCase(nameof(OptimizedClass.AsyncVoidMethod))]
    public async Task Retest_exception_case_in_an_async_method(string method)
    {
        mock.Setup(m => m.VoidMethod()).Throws(new Exception("test"));
        mock.Setup(m => m.AsyncVoidMethod()).ThrowsAsync(new Exception("test"));

        var testing = InvokerFor<OptimizedClass>(method);

        try
        {
            await testing.InvokeAsync(target);
            Assert.Fail("exception not thrown");
        }
        catch (Exception ex)
        {
            Assert.AreEqual("test", ex.Message);
        }
    }
}
