using Routine.Interception;

namespace Routine.Test.Interception.Stubs.Interceptors;

public class AsyncAround : AsyncAroundInterceptorBase<AsyncAround, Context>, IAroundInterceptor<AsyncAround>
{
    private readonly int delay;

    public AsyncAround() : this(1) { }
    public AsyncAround(int delay)
    {
        this.delay = delay;
    }

    private Exception exceptionBefore;
    public void FailOnBeforeWith(Exception exceptionBefore) => this.exceptionBefore = exceptionBefore;

    private object result;
    public void CancelAndReturn(object result) => this.result = result;

    private object resultOverride;
    public void OverrideActualResultWith(object resultOverride) => this.resultOverride = resultOverride;

    private Exception exceptionSuccess;
    public void FailOnSuccessWith(Exception exceptionSuccess) => this.exceptionSuccess = exceptionSuccess;

    private object resultOnFail;
    public void HideFailAndReturn(object resultOnFail) => this.resultOnFail = resultOnFail;

    private Exception exception;
    public void OverrideExceptionWith(Exception exception) => this.exception = exception;

    protected override async Task OnBefore(Context context)
    {
        await Task.Delay(delay);

        if (exceptionBefore != null)
        {
            throw exceptionBefore;
        }

        context["before"] = true;

        if (result != null)
        {
            context.Canceled = true;
            context.Result = result;
        }
    }

    protected override async Task OnSuccess(Context context)
    {
        await Task.Delay(delay);

        if (exceptionSuccess != null)
        {
            throw exceptionSuccess;
        }

        context["success"] = true;

        if (resultOverride != null)
        {
            context.Result = resultOverride;
        }
    }

    protected override async Task OnFail(Context context)
    {
        await Task.Delay(delay);

        context["fail"] = true;

        if (resultOnFail != null)
        {
            context.ExceptionHandled = true;
            context.Result = resultOnFail;
        }

        if (exception != null)
        {
            context.Exception = exception;
        }
    }

    protected override async Task OnAfter(Context context)
    {
        await Task.Delay(delay);

        context["after"] = true;
    }

    protected override bool CanIntercept(Context context) =>
        key != null && context[key] != null || base.CanIntercept(context);

    private string key;
    public AsyncAround WhenContextHas(string key)
    {
        this.key = key;

        return this;
    }
}
