using Routine.Interception;

namespace Routine.Test.Interception.Stubs.Invocations;

public class Sync : InvocationBase<object>
{
    protected override object Intercept(IInterceptor<Context> testing) => testing.Intercept(_context, _invocation);
    protected override object Convert(object result) => result;
}
