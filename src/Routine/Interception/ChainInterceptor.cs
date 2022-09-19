namespace Routine.Interception;

public class ChainInterceptor<TContext> : IInterceptor<TContext>
    where TContext : InterceptionContext
{
    private IChainLinkInterceptor<TContext> first;
    private IChainLinkInterceptor<TContext> last;

    public ChainInterceptor() : this(new List<IInterceptor<TContext>>()) { }
    public ChainInterceptor(IEnumerable<IInterceptor<TContext>> initialList)
    {
        foreach (var interceptor in initialList)
        {
            Add(interceptor);
        }
    }

    public void Add(IInterceptor<TContext> interceptor)
    {
        var newLink = new AdapterChainLinkInterceptor<TContext>(interceptor);

        if (first == null || last == null)
        {
            first = last = newLink;

            return;
        }

        last.Next = newLink;
        last = newLink;
    }

    public void Merge(ChainInterceptor<TContext> other)
    {
        if (other.first == null || other.last == null) { return; }

        if (first == null || last == null)
        {
            first = other.first;
            last = other.last;

            return;
        }

        last.Next = other.first;
        last = other.last;
    }

    private async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation) =>
        first == null
            ? await invocation()
            : await first.InterceptAsync(context, invocation);

    #region IInterceptor<TContext> implementation

    async Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => await InterceptAsync(context, invocation);

    #endregion
}
