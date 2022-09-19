namespace Routine.Interception;

public abstract class InterceptorBase<TContext> : IInterceptor<TContext>
    where TContext : InterceptionContext
{
    protected abstract Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation);

    async Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => await InterceptAsync(context, invocation);
}
