using System;
using Routine.Engine.Locator;

namespace Routine.Engine.Configuration
{
	public class LocatorBuilder
	{
		public DelegateBasedLocator By(Func<IType, string, object> locatorDelegate)
		{
			return new DelegateBasedLocator(locatorDelegate);
		}

		//first level facade
		public DelegateBasedLocator Singleton(Func<IType, object> locatorDelegate)
		{
			return By((t, id) => locatorDelegate(t));
		}

		public DelegateBasedLocator Constant(object staticResult)
		{
			return By((t, id) => staticResult);
		}

		public DelegateBasedLocator By(Func<string, object> convertDelegate)
		{
			return By((t, id) => convertDelegate(id));
		}
	}
}
