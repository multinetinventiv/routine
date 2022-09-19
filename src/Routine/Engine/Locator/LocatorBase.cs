namespace Routine.Engine.Locator;

public abstract class LocatorBase<TConcrete> : ILocator
    where TConcrete : LocatorBase<TConcrete>
{
    private bool acceptNullResult;

    protected LocatorBase()
    {
        AcceptNullResult(true);
    }

    public TConcrete AcceptNullResult(bool acceptNullResult) { this.acceptNullResult = acceptNullResult; return (TConcrete)this; }

    private async Task<List<object>> LocateInnerAsync(IType type, List<string> ids)
    {
        var result = await LocateAsync(type, ids) ?? new();

        if (!acceptNullResult && result.Contains(null))
        {
            throw new CannotLocateException(type, ids);
        }

        if (result.Count != ids.Count)
        {
            throw new CannotLocateException(type, ids,
                new InvalidOperationException(
                    $"Locate result count ({result.Count}) cannot be different than id count ({ids.Count})")
            );
        }

        return result;
    }

    protected abstract Task<List<object>> LocateAsync(IType type, List<string> ids);

    #region ILocator implementation

    async Task<List<object>> ILocator.LocateAsync(IType type, List<string> ids) => await LocateInnerAsync(type, ids);

    #endregion
}
