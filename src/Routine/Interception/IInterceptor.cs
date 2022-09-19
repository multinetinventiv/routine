using Routine.Core.Runtime;

namespace Routine.Interception;

public interface IInterceptor<in TContext>
    where TContext : InterceptionContext
{
    Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation);

    public object Intercept(TContext context, Func<object> invocation) => InterceptAsync(context, () => Task.FromResult(invocation())).WaitAndGetResult();
}
