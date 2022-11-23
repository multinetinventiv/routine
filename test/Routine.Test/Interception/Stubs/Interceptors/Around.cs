using Routine.Interception;

namespace Routine.Test.Interception.Stubs.Interceptors;

public class Around : AroundInterceptorBase<Around, Context>
{
    private readonly int _delay;

    public Around() : this(1) { }
    public Around(int delay)
    {
        _delay = delay;
    }

    private Exception _exceptionBefore;
    public void FailOnBeforeWith(Exception exceptionBefore) => _exceptionBefore = exceptionBefore;

    private object _result;
    public void CancelAndReturn(object result) => _result = result;

    private object _resultOverride;
    public void OverrideActualResultWith(object resultOverride) => _resultOverride = resultOverride;

    private Exception _exceptionSuccess;
    public void FailOnSuccessWith(Exception exceptionSuccess) => _exceptionSuccess = exceptionSuccess;

    private object _resultOnFail;
    public void HideFailAndReturn(object resultOnFail) => _resultOnFail = resultOnFail;

    private Exception _exception;
    public void OverrideExceptionWith(Exception exception) => _exception = exception;

    protected override async Task OnBefore(Context context)
    {
        await Task.Delay(_delay);

        if (_exceptionBefore != null)
        {
            throw _exceptionBefore;
        }

        context["before"] = true;

        if (_result != null)
        {
            context.Canceled = true;
            context.Result = _result;
        }
    }

    protected override async Task OnSuccess(Context context)
    {
        await Task.Delay(_delay);

        if (_exceptionSuccess != null)
        {
            throw _exceptionSuccess;
        }

        context["success"] = true;

        if (_resultOverride != null)
        {
            context.Result = _resultOverride;
        }
    }

    protected override async Task OnFail(Context context)
    {
        await Task.Delay(_delay);

        context["fail"] = true;

        if (_resultOnFail != null)
        {
            context.ExceptionHandled = true;
            context.Result = _resultOnFail;
        }

        if (_exception != null)
        {
            context.Exception = _exception;
        }
    }

    protected override async Task OnAfter(Context context)
    {
        await Task.Delay(_delay);

        context["after"] = true;
    }

    protected override bool CanIntercept(Context context) =>
        _key != null && context[_key] != null || base.CanIntercept(context);

    private string _key;
    public Around WhenContextHas(string key)
    {
        _key = key;

        return this;
    }
}
