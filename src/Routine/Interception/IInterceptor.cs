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
}
