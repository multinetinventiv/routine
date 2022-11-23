namespace Routine.Engine.Locator;

public class SingleDelegateLocator : LocatorBase<SingleDelegateLocator>
{
    private Func<IType, string, object> _locatorDelegate;

    public SingleDelegateLocator(Func<IType, string, object> locatorDelegate)
    {
        _locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        Task.FromResult(ids.Select(id => _locatorDelegate(type, id)).ToList());
}
