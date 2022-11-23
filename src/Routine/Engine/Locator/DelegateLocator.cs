namespace Routine.Engine.Locator;

public class DelegateLocator : LocatorBase<DelegateLocator>
{
    private Func<IType, List<string>, IEnumerable> _locatorDelegate;

    public DelegateLocator(Func<IType, List<string>, IEnumerable> locatorDelegate)
    {
        _locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        Task.FromResult(_locatorDelegate(type, ids).Cast<object>().ToList());
}
