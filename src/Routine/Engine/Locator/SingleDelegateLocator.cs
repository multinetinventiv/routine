namespace Routine.Engine.Locator;

public class SingleDelegateLocator : LocatorBase<SingleDelegateLocator>
{
    private Func<IType, string, object> locatorDelegate;

    public SingleDelegateLocator(Func<IType, string, object> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        Task.FromResult(ids.Select(id => locatorDelegate(type, id)).ToList());
}
