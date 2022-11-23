namespace Routine.Core.Configuration.Convention;

public abstract class ConventionBase<TFrom, TResult> : IConvention<TFrom, TResult>
{
    private Func<TFrom, bool> _whenDelegate;

    protected ConventionBase()
    {
        _whenDelegate = _ => true;
    }

    public IConvention<TFrom, TResult> WhenDefault() => When(default(TFrom));
    public IConvention<TFrom, TResult> When(TFrom expected) => When(o => Equals(o, expected));

    public ConventionBase<TFrom, TResult> When(Func<TFrom, bool> whenDelegate)
    {
        _whenDelegate = _whenDelegate.And(whenDelegate);

        return this;
    }

    protected virtual bool AppliesTo(TFrom obj) => _whenDelegate(obj);

    private TResult SafeApply(TFrom obj)
    {
        if (!AppliesTo(obj)) { throw new ConfigurationException(obj); }

        return Apply(obj);
    }

    protected abstract TResult Apply(TFrom obj);

    #region IConvention implementation

    bool IConvention<TFrom, TResult>.AppliesTo(TFrom obj) => AppliesTo(obj);
    TResult IConvention<TFrom, TResult>.Apply(TFrom obj) => SafeApply(obj);

    #endregion
}
