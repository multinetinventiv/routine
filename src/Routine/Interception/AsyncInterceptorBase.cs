using Routine.Core.Runtime;
using System.Threading.Tasks;
using System;

namespace Routine.Interception
{
    public abstract class AsyncInterceptorBase<TContext> : IInterceptor<TContext>
        where TContext : InterceptionContext
    {
        protected abstract Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation);

        object IInterceptor<TContext>.Intercept(TContext context, Func<object> invocation) => InterceptAsync(context, () => Task.FromResult(invocation())).WaitAndGetResult();
        async Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => await InterceptAsync(context, invocation);
    }
}
