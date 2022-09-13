namespace Routine.Core.Configuration.Convention;

public class DelegateBasedConvention<TFrom, TResult> : ConventionBase<TFrom, TResult>
    where TFrom : notnull
    where TResult : notnull
{
    private Func<TFrom?, TResult?> converterDelegate;

    public DelegateBasedConvention()
    {
        converterDelegate = _ => default;
    }

    public DelegateBasedConvention<TFrom, TResult> Return(Func<TFrom?, TResult?> converterDelegate)
    {
        this.converterDelegate = converterDelegate;

        return this;
    }

    protected override TResult? Apply(TFrom? obj) => converterDelegate(obj);
}
