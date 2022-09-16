namespace Routine.Engine.Locator;

public class SingletonLocator : LocatorBase<SingletonLocator>
{
    private Func<IType, object> locatorDelegate;

    public SingletonLocator(Func<IType, object> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate;
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
    {
        var result = locatorDelegate(type);

        return Task.FromResult(ids.Select(_ => result).ToList());
    }
}
