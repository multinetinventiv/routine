namespace Routine.Interception;

public class DecoratorInterceptor<TContext, TVariable> : AroundInterceptorBase<DecoratorInterceptor<TContext, TVariable>, TContext>
    where TContext : InterceptionContext
{
    private static readonly Action<TContext, TVariable> EMPTY = (_, _) => { };
    private static Action<TContext, TVariable> Wrap(Action<TVariable> @delegate) => @delegate == null ? null : (_, obj) => @delegate(obj);

    private readonly Func<TContext, TVariable> beforeDelegate;
    private readonly string variableName;

    private Action<TContext, TVariable> successDelegate = EMPTY;
    private Action<TContext, TVariable> failDelegate = EMPTY;
    private Action<TContext, TVariable> afterDelegate = EMPTY;


    public DecoratorInterceptor(Func<TVariable> beforeDelegate) : this(_ => beforeDelegate()) { }
    public DecoratorInterceptor(Func<TContext, TVariable> beforeDelegate)
    {
        this.beforeDelegate = beforeDelegate ?? throw new ArgumentNullException(nameof(beforeDelegate));

        variableName = DecoratorInterceptorVariableNameFactory.NextVariableName();
    }

    public DecoratorInterceptor<TContext, TVariable> Success(Action<TVariable> successDelegate) => Success(Wrap(successDelegate));
    public DecoratorInterceptor<TContext, TVariable> Success(Action<TContext, TVariable> successDelegate) { this.successDelegate = successDelegate ?? this.successDelegate; return this; }

    public DecoratorInterceptor<TContext, TVariable> Fail(Action<TVariable> failDelegate) => Fail(Wrap(failDelegate));
    public DecoratorInterceptor<TContext, TVariable> Fail(Action<TContext, TVariable> failDelegate) { this.failDelegate = failDelegate ?? this.failDelegate; return this; }

    public DecoratorInterceptor<TContext, TVariable> After(Action<TVariable> afterDelegate) => After(Wrap(afterDelegate));
    public DecoratorInterceptor<TContext, TVariable> After(Action<TContext, TVariable> afterDelegate) { this.afterDelegate = afterDelegate ?? this.afterDelegate; return this; }

    private string ExceptionVariableName => variableName + "_exception";
    private bool ExceptionOccuredOnBefore(TContext context) => context[ExceptionVariableName] != null;

    protected override void OnBefore(TContext context)
    {
        try
        {
            context[variableName] = beforeDelegate(context);
        }
        catch (Exception ex)
        {
            context[ExceptionVariableName] = ex;
            throw;
        }
    }

    protected override void OnSuccess(TContext context)
    {
        if (ExceptionOccuredOnBefore(context)) { return; }

        successDelegate(context, (TVariable)context[variableName]);
    }

    protected override void OnFail(TContext context)
    {
        if (ExceptionOccuredOnBefore(context)) { return; }

        failDelegate(context, (TVariable)context[variableName]);
    }

    protected override void OnAfter(TContext context)
    {
        if (ExceptionOccuredOnBefore(context)) { return; }

        afterDelegate(context, (TVariable)context[variableName]);
    }
}
