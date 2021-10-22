using Routine.Core.Runtime;
using System;
using System.Threading.Tasks;

namespace Routine.Interception
{
    public interface IInterceptor<in TContext>
        where TContext : InterceptionContext
    {
        object Intercept(TContext context, Func<object> invocation);
        Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation);
    }

    public abstract class InterceptorBase<TContext> : IInterceptor<TContext>
        where TContext : InterceptionContext
    {
        protected abstract object Intercept(TContext context, Func<object> invocation);

        object IInterceptor<TContext>.Intercept(TContext context, Func<object> invocation) => Intercept(context, invocation);
        Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => Task.FromResult(Intercept(context, () => invocation().WaitAndGetResult()));
    }

    public abstract class AsyncInterceptorBase<TContext> : IInterceptor<TContext>
        where TContext : InterceptionContext
    {
        protected abstract Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation);

        object IInterceptor<TContext>.Intercept(TContext context, Func<object> invocation) => InterceptAsync(context, () => Task.FromResult(invocation())).WaitAndGetResult();
        async Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => await InterceptAsync(context, invocation);
    }
}
