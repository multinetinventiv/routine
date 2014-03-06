using System;
using Routine.Core.Locator;

namespace Routine.Core.Builder
{
	public class LocatorBuilder
	{
		public DelegateLocator Singleton(Func<TypeInfo, object> locatorDelegate)
		{
			return By((t, id) => locatorDelegate(t));
		}
		
		public DelegateLocator Always(object staticResult)
		{
			return By((t, id) => staticResult);
		}

		public DelegateLocator ByConverting(Func<string, object> convertDelegate)
		{
			return By((t, id) => convertDelegate(id));
		}

		public DelegateLocator By(Func<TypeInfo, string, object> locatorDelegate)
		{
			return new DelegateLocator(locatorDelegate);
		}

		//first level facade
		public DelegateLocator Directly()
		{
			return ByConverting(id => id);
		}
	}
}
