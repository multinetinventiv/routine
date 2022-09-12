using Routine.Engine.Locator;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Routine.Engine.Configuration;

public class LocatorBuilder
{
    public DelegateBasedLocator By(Func<IType, List<string>, IEnumerable> locatorDelegate) => new((t, ids) => locatorDelegate(t, ids).Cast<object>().ToList());
    public DelegateBasedLocator SingleBy(Func<IType, string, object> locatorDelegate) => new((t, ids) => ids.Select(id => locatorDelegate(t, id)).ToList());
    public DelegateBasedLocator Singleton(Func<IType, object> locatorDelegate) => SingleBy((t, _) => locatorDelegate(t));
    public DelegateBasedLocator Constant(object staticResult) => SingleBy((_, _) => staticResult);
    public DelegateBasedLocator SingleBy(Func<string, object> convertDelegate) => SingleBy((_, id) => convertDelegate(id));
    public DelegateBasedLocator By(Func<List<string>, IEnumerable> convertDelegate) => By((_, id) => convertDelegate(id));
}
