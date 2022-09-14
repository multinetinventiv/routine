using Routine.Interception;

namespace Routine.Test.Interception.Stubs.Invocations;

public abstract class InvocationBase<TResult> : IInvocation
{
    protected Context context;
    private int count;
    private object result;
    private Exception exception;
    protected Func<TResult> invocation;

    protected InvocationBase()
    {
        context = new Context();
        count = 0;
        result = null;
        exception = null;
        invocation = () =>
        {
            count++;

            if (exception != null) { throw exception; }

            context["invocation"] = true;

            return Convert(result);
        };
    }

    public Context Context => context;
    public int Count => count;
    public string ExceptionStackTraceLookupText => "InvocationBase`1.<.ctor>";

    public void Returns(object result) => this.result = result;
    public void FailsWith(Exception exception) => this.exception = exception;

    protected abstract object Intercept(IInterceptor<Context> testing);
    protected abstract TResult Convert(object result);

    object IInvocation.Intercept(IInterceptor<Context> testing) => Intercept(testing);
}
