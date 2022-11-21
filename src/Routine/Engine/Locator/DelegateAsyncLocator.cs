namespace Routine.Engine.Locator;

public class DelegateAsyncLocator : LocatorBase<DelegateAsyncLocator>
{
    private readonly Func<IType, List<string>, Task<IEnumerable>> _locatorDelegate;

    public DelegateAsyncLocator(Func<IType, List<string>, Task<IEnumerable>> locatorDelegate)
    {
        _locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override async Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        (await _locatorDelegate(type, ids)).Cast<object>().ToList();
}
