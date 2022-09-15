namespace Routine.Engine.Locator;

public class DelegateLocator : LocatorBase<DelegateLocator>
{
    private static Func<IType, List<string>, Task<IEnumerable>> Wrap(Func<IType, string, Task<object>> locatorDelegate)
    {
        locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));

        return async (t, ids) => await Task.WhenAll(ids.Select(id => locatorDelegate(t, id)));
    }

    private readonly Func<IType, List<string>, Task<IEnumerable>> locatorDelegate;

    public DelegateLocator(Func<IType, string, Task<object>> locatorDelegate) : this(Wrap(locatorDelegate)) { }
    public DelegateLocator(Func<IType, List<string>, Task<IEnumerable>> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override async Task<List<object>> LocateAsync(IType type, List<string> ids) => (await locatorDelegate(type, ids)).Cast<object>().ToList();
}
