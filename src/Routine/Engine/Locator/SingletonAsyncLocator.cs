namespace Routine.Engine.Locator;

public class SingletonAsyncLocator : LocatorBase<SingletonAsyncLocator>
{
    private Func<IType, Task<object>> locatorDelegate;

    public SingletonAsyncLocator(Func<IType, Task<object>> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate;
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
    {
        throw new NotImplementedException();
    }
}
