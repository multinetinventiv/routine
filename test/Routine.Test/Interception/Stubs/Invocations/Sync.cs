using Routine.Interception;

namespace Routine.Test.Interception.Stubs.Invocations
{
    public class Sync : InvocationBase<object>
    {
        protected override object Intercept(IInterceptor<TestContext> testing) => testing.Intercept(context, invocation);
        protected override object Convert(object result) => result;
    }
}