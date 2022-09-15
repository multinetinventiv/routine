using Routine.Engine.Locator;

namespace Routine.Engine.Configuration;

public class LocatorBuilder
{
    public ConstantLocator Constant(object staticResult) => new(staticResult);

    public SingletonLocator Singleton(Func<IType, object> locatorDelegate) => new(locatorDelegate);
    public SingletonAsyncLocator Singleton(Func<IType, Task<object>> locatorDelegate) => new(locatorDelegate);

    public SingleDelegateLocator SingleBy(Func<string, object> convertDelegate) => SingleBy((_, id) => convertDelegate(id));
    public SingleDelegateLocator SingleBy(Func<IType, string, object> locatorDelegate) => new(locatorDelegate);
    public SingleDelegateAsyncLocator SingleBy(Func<string, Task<object>> convertDelegate) => SingleBy(async (_, id) => await convertDelegate(id));
    public SingleDelegateAsyncLocator SingleBy(Func<IType, string, Task<object>> locatorDelegate) => new(locatorDelegate);

    public DelegateLocator By(Func<List<string>, IEnumerable> convertDelegate) => By((_, ids) => convertDelegate(ids));
    public DelegateLocator By(Func<IType, List<string>, IEnumerable> locatorDelegate) => new(locatorDelegate);
    public DelegateAsyncLocator By(Func<List<string>, Task<IEnumerable>> convertDelegate) => By(async (_, ids) => await convertDelegate(ids));
    public DelegateAsyncLocator By(Func<IType, List<string>, Task<IEnumerable>> locatorDelegate) => new(locatorDelegate);
}
