using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Locator;

namespace Routine.Engine.Configuration
{
	public class LocatorBuilder
	{
		public DelegateBasedLocator By(Func<IType, List<string>, IEnumerable> locatorDelegate)
		{
			return new DelegateBasedLocator((t, ids) => locatorDelegate(t, ids).Cast<object>().ToList());
		}

		public DelegateBasedLocator SingleBy(Func<IType, string, object> locatorDelegate)
		{
			return new DelegateBasedLocator((t, ids) => ids.Select(id => locatorDelegate(t, id)).ToList());
		}

		public DelegateBasedLocator Singleton(Func<IType, object> locatorDelegate)
		{
			return SingleBy((t, _) => locatorDelegate(t));
		}

		public DelegateBasedLocator Constant(object staticResult)
		{
			return SingleBy((_, _) => staticResult);
		}

		public DelegateBasedLocator SingleBy(Func<string, object> convertDelegate)
		{
			return SingleBy((_, id) => convertDelegate(id));
		}

		public DelegateBasedLocator By(Func<List<string>, IEnumerable> convertDelegate)
		{
			return By((_, id) => convertDelegate(id));
		}
	}
}
