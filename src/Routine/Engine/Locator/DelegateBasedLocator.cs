using System.Collections.Generic;
using System;

namespace Routine.Engine.Locator;

public class DelegateBasedLocator : LocatorBase<DelegateBasedLocator>
{
    private readonly Func<IType, List<string>, List<object>> locatorDelegate;

    public DelegateBasedLocator(Func<IType, List<string>, List<object>> locatorDelegate)
    {
        this.locatorDelegate = locatorDelegate ?? throw new ArgumentNullException(nameof(locatorDelegate));
    }

    protected override List<object> Locate(IType type, List<string> ids) => locatorDelegate(type, ids);
}
