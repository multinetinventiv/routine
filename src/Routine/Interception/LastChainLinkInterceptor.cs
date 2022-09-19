namespace Routine.Interception;

public class LastChainLinkInterceptor<TContext> : IChainLinkInterceptor<TContext>
    where TContext : InterceptionContext
{
    public IChainLinkInterceptor<TContext> Next { get => null; set { } }

    public async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation) => await invocation();
}
