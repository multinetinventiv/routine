namespace Routine.Engine.Locator;

public class SingletonLocator : LocatorBase<SingletonLocator>
{
    private Func<IType, object> _locatorDelegate;

    public SingletonLocator(Func<IType, object> locatorDelegate)
    {
        _locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
    {
        var result = _locatorDelegate(type);

        return Task.FromResult(ids.Select(_ => result).ToList());
    }
}
