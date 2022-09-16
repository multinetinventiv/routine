using Routine.Engine.Locator;

namespace Routine.Engine.Configuration;

public class LocatorBuilder
{
    public ConstantLocator Constant(object constant) => new(constant);

    public SingletonLocator Singleton(Func<IType, object> locatorDelegate) => new(locatorDelegate);
    public SingletonAsyncLocator Singleton(Func<IType, Task<object>> locatorDelegate) => new(locatorDelegate);

    public SingleDelegateLocator SingleBy(Func<string, object> locatorDelegate) => SingleBy((_, id) => locatorDelegate(id));
    public SingleDelegateLocator SingleBy(Func<IType, string, object> locatorDelegate) => new(locatorDelegate);
    public SingleDelegateAsyncLocator SingleBy(Func<string, Task<object>> locatorDelegate) => SingleBy(async (_, id) => await locatorDelegate(id));
    public SingleDelegateAsyncLocator SingleBy(Func<IType, string, Task<object>> locatorDelegate) => new(locatorDelegate);

    public DelegateLocator By(Func<List<string>, IEnumerable> locatorDelegate) => By((_, ids) => locatorDelegate(ids));
    public DelegateLocator By(Func<IType, List<string>, IEnumerable> locatorDelegate) => new(locatorDelegate);
    public DelegateAsyncLocator By(Func<List<string>, Task<IEnumerable>> locatorDelegate) => By(async (_, ids) => await locatorDelegate(ids));
    public DelegateAsyncLocator By(Func<IType, List<string>, Task<IEnumerable>> locatorDelegate) => new(locatorDelegate);
}
