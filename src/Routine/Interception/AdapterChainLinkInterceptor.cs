namespace Routine.Interception;

public class AdapterChainLinkInterceptor<TContext> : IChainLinkInterceptor<TContext>
    where TContext : InterceptionContext
{
    private readonly IInterceptor<TContext> real;
    private IChainLinkInterceptor<TContext> next;

    public AdapterChainLinkInterceptor(IInterceptor<TContext> real)
    {
        this.real = real;

        next = new LastChainLinkInterceptor<TContext>();
    }

    public IChainLinkInterceptor<TContext> Next
    {
        get => next;
        set => next = value ?? new LastChainLinkInterceptor<TContext>();
    }

    public async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation) => await real.InterceptAsync(context, async () => await next.InterceptAsync(context, invocation));
}
