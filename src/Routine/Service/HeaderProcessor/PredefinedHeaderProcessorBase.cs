namespace Routine.Service.HeaderProcessor;

public abstract class PredefinedHeaderProcessorBase<TConcrete> : IHeaderProcessor
    where TConcrete : PredefinedHeaderProcessorBase<TConcrete>
{
    private readonly string[] _headerKeys;
    private Action<List<string>> _processorDelegate;

    protected PredefinedHeaderProcessorBase(params string[] headerKeys)
    {
        _headerKeys = headerKeys;

        _processorDelegate = _ => { };
    }

    protected void Process(IDictionary<string, string> responseHeaders)
    {
        var headers = new List<string>();
        foreach (var headerKey in _headerKeys)
        {
            if (!responseHeaders.TryGetValue(headerKey, out var header))
            {
                header = string.Empty;
            }

            headers.Add(header);
        }

        _processorDelegate(headers);
    }

    protected TConcrete Do(Action<List<string>> processorDelegate) { _processorDelegate = processorDelegate; return (TConcrete)this; }

    #region IHeaderProcessor implementation

    void IHeaderProcessor.Process(IDictionary<string, string> responseHeaders) => Process(responseHeaders);

    #endregion
}
