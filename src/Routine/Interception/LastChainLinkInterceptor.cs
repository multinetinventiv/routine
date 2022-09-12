using System.Threading.Tasks;
using System;

namespace Routine.Interception;

public class LastChainLinkInterceptor<TContext> : IChainLinkInterceptor<TContext>
    where TContext : InterceptionContext
{
    public IChainLinkInterceptor<TContext> Next { get => null; set { } }

    public object Intercept(TContext context, Func<object> invocation) => invocation();
    public async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation) => await invocation();
}
