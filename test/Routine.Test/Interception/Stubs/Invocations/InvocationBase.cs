using Routine.Interception;

namespace Routine.Test.Interception.Stubs.Invocations;

public abstract class InvocationBase<TResult> : IInvocation
{
    protected Context _context;
    private int _count;
    private object _result;
    private Exception _exception;
    protected Func<TResult> _invocation;

    protected InvocationBase()
    {
        _context = new();
        _count = 0;
        _result = null;
        _exception = null;
        _invocation = () =>
        {
            _count++;

            if (_exception != null) { throw _exception; }

            _context["invocation"] = true;

            return Convert(_result);
        };
    }

    public Context Context => _context;
    public int Count => _count;
    public string ExceptionStackTraceLookupText => "InvocationBase`1.<.ctor>";

    public void Returns(object result) => _result = result;
    public void FailsWith(Exception exception) => _exception = exception;

    protected abstract object Intercept(IInterceptor<Context> testing);
    protected abstract TResult Convert(object result);

    object IInvocation.Intercept(IInterceptor<Context> testing) => Intercept(testing);
}
