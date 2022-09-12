using System.Collections.Generic;
using System;

namespace Routine.Service.HeaderProcessor;

public abstract class PredefinedHeaderProcessorBase<TConcrete> : IHeaderProcessor
    where TConcrete : PredefinedHeaderProcessorBase<TConcrete>
{
    private readonly string[] headerKeys;
    private Action<List<string>> processorDelegate;

    protected PredefinedHeaderProcessorBase(params string[] headerKeys)
    {
        this.headerKeys = headerKeys;

        processorDelegate = _ => { };
    }

    protected void Process(IDictionary<string, string> responseHeaders)
    {
        var headers = new List<string>();
        foreach (var headerKey in headerKeys)
        {
            if (!responseHeaders.TryGetValue(headerKey, out var header))
            {
                header = string.Empty;
            }

            headers.Add(header);
        }

        processorDelegate(headers);
    }

    protected TConcrete Do(Action<List<string>> processorDelegate) { this.processorDelegate = processorDelegate; return (TConcrete)this; }

    #region IHeaderProcessor implementation

    void IHeaderProcessor.Process(IDictionary<string, string> responseHeaders) => Process(responseHeaders);

    #endregion
}
