namespace Routine.Engine.Locator;

public class DelegateAsyncLocator : LocatorBase<DelegateAsyncLocator>
{
    private readonly Func<IType, List<string>, Task<IEnumerable>> locatorDelegate;

    public DelegateAsyncLocator(Func<IType, List<string>, Task<IEnumerable>> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override async Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        (await locatorDelegate(type, ids)).Cast<object>().ToList();
}
