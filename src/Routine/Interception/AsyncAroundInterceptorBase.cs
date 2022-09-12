using System.Threading.Tasks;
using System;

namespace Routine.Interception;

public abstract class AsyncAroundInterceptorBase<TConcrete, TContext> : AsyncInterceptorBase<TContext>
    where TConcrete : AsyncAroundInterceptorBase<TConcrete, TContext>
    where TContext : InterceptionContext
{
    private Func<TContext, bool> whenDelegate;

    protected AsyncAroundInterceptorBase()
    {
        When(_ => true);
    }

    public TConcrete When(Func<TContext, bool> whenDelegate) { this.whenDelegate = whenDelegate; return (TConcrete)this; }

    protected virtual bool CanIntercept(TContext context) => whenDelegate(context);

    protected override async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation)
    {
        if (!CanIntercept(context)) { return await invocation(); }

        try
        {
            await OnBefore(context);

            if (!context.Canceled)
            {
                context.Result = await invocation();
            }

            await OnSuccess(context);
        }
        catch (Exception ex)
        {
            context.Exception = ex;
            await OnFail(context);
            if (!context.ExceptionHandled)
            {
                if (ex == context.Exception) // if exception was not changed, preserve stack trace
                {
                    throw;
                }
                else
                {
                    throw context.Exception;
                }
            }
        }
        finally
        {
            await OnAfter(context);
        }

        return context.Result;
    }

    protected abstract Task OnBefore(TContext context);
    protected abstract Task OnSuccess(TContext context);
    protected abstract Task OnFail(TContext context);
    protected abstract Task OnAfter(TContext context);
}
