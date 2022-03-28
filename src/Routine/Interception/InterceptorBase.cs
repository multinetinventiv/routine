using Routine.Core.Runtime;
using System.Threading.Tasks;
using System;

namespace Routine.Interception
{
    public abstract class InterceptorBase<TContext> : IInterceptor<TContext>
        where TContext : InterceptionContext
    {
        protected abstract object Intercept(TContext context, Func<object> invocation);

        object IInterceptor<TContext>.Intercept(TContext context, Func<object> invocation) => Intercept(context, invocation);
        Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => Task.FromResult(Intercept(context, () => invocation().WaitAndGetResult()));
    }
}
