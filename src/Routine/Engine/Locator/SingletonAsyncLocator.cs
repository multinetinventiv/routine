namespace Routine.Engine.Locator;

public class SingletonAsyncLocator : LocatorBase<SingletonAsyncLocator>
{
    private Func<IType, Task<object>> _locatorDelegate;

    public SingletonAsyncLocator(Func<IType, Task<object>> locatorDelegate)
    {
        _locatorDelegate = locatorDelegate;
    }

    protected override async Task<List<object>> LocateAsync(IType type, List<string> ids)
    {
        var result = await _locatorDelegate(type);

        return ids.Select(_ => result).ToList();
    }
}
