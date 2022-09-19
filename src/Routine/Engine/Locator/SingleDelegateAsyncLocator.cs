namespace Routine.Engine.Locator;

public class SingleDelegateAsyncLocator : LocatorBase<SingleDelegateAsyncLocator>
{
    private Func<IType, string, Task<object>> locatorDelegate;

    public SingleDelegateAsyncLocator(Func<IType, string, Task<object>> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override async Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        (await Task.WhenAll(ids.Select(id => locatorDelegate(type, id)))).ToList();
}
