namespace Routine.Interception;

public class AdapterChainLinkInterceptor<TContext> : IChainLinkInterceptor<TContext>
    where TContext : InterceptionContext
{
    private readonly IInterceptor<TContext> _real;
    private IChainLinkInterceptor<TContext> _next;

    public AdapterChainLinkInterceptor(IInterceptor<TContext> real)
    {
        _real = real;

        _next = new LastChainLinkInterceptor<TContext>();
    }

    public IChainLinkInterceptor<TContext> Next
    {
        get => _next;
        set => _next = value ?? new LastChainLinkInterceptor<TContext>();
    }

    public async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation) => await _real.InterceptAsync(context, async () => await _next.InterceptAsync(context, invocation));
}
