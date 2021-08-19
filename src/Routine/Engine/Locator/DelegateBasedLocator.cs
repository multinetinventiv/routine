using System;
using System.Collections.Generic;

namespace Routine.Engine.Locator
{
	public class DelegateBasedLocator : LocatorBase<DelegateBasedLocator>
	{
		private readonly Func<IType, List<string>, List<object>> locatorDelegate;

		public DelegateBasedLocator(Func<IType, List<string>, List<object>> locatorDelegate)
		{
			if (locatorDelegate == null) { throw new ArgumentNullException(nameof(locatorDelegate)); }

			this.locatorDelegate = locatorDelegate;
		}

		protected override List<object> Locate(IType type, List<string> ids)
		{
			return locatorDelegate(type, ids);
		}
	}
}
