namespace Routine.Interception;

public class AroundInterceptor<TContext> : AroundInterceptorBase<AroundInterceptor<TContext>, TContext>
    where TContext : InterceptionContext
{
    private static readonly Func<TContext, Task> EMPTY = _ => Task.CompletedTask;
    private static Func<TContext, Task> Wrap(Action @delegate) => @delegate == null ? null : _ => { @delegate(); return Task.CompletedTask; };
    private static Func<TContext, Task> Wrap(Action<TContext> @delegate) => @delegate == null ? null : ctx => { @delegate(ctx); return Task.CompletedTask; };
    private static Func<TContext, Task> Wrap(Func<Task> @delegate) => @delegate == null ? null : async _ => await @delegate();

    private Func<TContext, Task> _beforeDelegate = EMPTY;
    private Func<TContext, Task> _successDelegate = EMPTY;
    private Func<TContext, Task> _failDelegate = EMPTY;
    private Func<TContext, Task> _afterDelegate = EMPTY;

    public AroundInterceptor<TContext> Before(Action beforeDelegate) => Before(Wrap(beforeDelegate));
    public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Before(Wrap(beforeDelegate));
    public AroundInterceptor<TContext> Before(Func<Task> beforeDelegate) => Before(Wrap(beforeDelegate));
    public AroundInterceptor<TContext> Before(Func<TContext, Task> beforeDelegate) { _beforeDelegate = beforeDelegate ?? _beforeDelegate; return this; }

    public AroundInterceptor<TContext> Success(Action successDelegate) => Success(Wrap(successDelegate));
    public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Success(Wrap(successDelegate));
    public AroundInterceptor<TContext> Success(Func<Task> successDelegate) => Success(Wrap(successDelegate));
    public AroundInterceptor<TContext> Success(Func<TContext, Task> successDelegate) { _successDelegate = successDelegate ?? _successDelegate; return this; }

    public AroundInterceptor<TContext> Fail(Action failDelegate) => Fail(Wrap(failDelegate));
    public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Fail(Wrap(failDelegate));
    public AroundInterceptor<TContext> Fail(Func<Task> failDelegate) => Fail(Wrap(failDelegate));
    public AroundInterceptor<TContext> Fail(Func<TContext, Task> failDelegate) { _failDelegate = failDelegate ?? _failDelegate; return this; }

    public AroundInterceptor<TContext> After(Action afterDelegate) => After(Wrap(afterDelegate));
    public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) => After(Wrap(afterDelegate));
    public AroundInterceptor<TContext> After(Func<Task> afterDelegate) => After(Wrap(afterDelegate));
    public AroundInterceptor<TContext> After(Func<TContext, Task> afterDelegate) { _afterDelegate = afterDelegate ?? _afterDelegate; return this; }

    protected override async Task OnBefore(TContext context) => await _beforeDelegate(context);
    protected override async Task OnSuccess(TContext context) => await _successDelegate(context);
    protected override async Task OnFail(TContext context) => await _failDelegate(context);
    protected override async Task OnAfter(TContext context) => await _afterDelegate(context);
}
