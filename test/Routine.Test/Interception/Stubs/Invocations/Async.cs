using Routine.Core.Runtime;
using Routine.Interception;
using System.Threading.Tasks;

namespace Routine.Test.Interception.Stubs.Invocations
{
    public class Async : InvocationBase<Task<object>>
    {
        protected override object Intercept(IInterceptor<TestContext> testing) => testing.InterceptAsync(context, invocation).WaitAndGetResult();
        protected override Task<object> Convert(object result) => Task.FromResult(result);
    }
}