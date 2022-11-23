namespace Routine.Interception;

public class ChainInterceptor<TContext> : IInterceptor<TContext>
    where TContext : InterceptionContext
{
    private IChainLinkInterceptor<TContext> _first;
    private IChainLinkInterceptor<TContext> _last;

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

        if (_first == null || _last == null)
        {
            _first = _last = newLink;

            return;
        }

        _last.Next = newLink;
        _last = newLink;
    }

    public void Merge(ChainInterceptor<TContext> other)
    {
        if (other._first == null || other._last == null) { return; }

        if (_first == null || _last == null)
        {
            _first = other._first;
            _last = other._last;

            return;
        }

        _last.Next = other._first;
        _last = other._last;
    }

    private async Task<object> InterceptAsync(TContext context, Func<Task<object>> invocation) =>
        _first == null
            ? await invocation()
            : await _first.InterceptAsync(context, invocation);

    #region IInterceptor<TContext> implementation

    async Task<object> IInterceptor<TContext>.InterceptAsync(TContext context, Func<Task<object>> invocation) => await InterceptAsync(context, invocation);

    #endregion
}
