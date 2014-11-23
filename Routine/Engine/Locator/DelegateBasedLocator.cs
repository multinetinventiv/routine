using System;

namespace Routine.Engine.Locator
{
	public class DelegateBasedLocator : BaseLocator<DelegateBasedLocator>
	{
		private readonly Func<IType, string, object> locatorDelegate;

		public DelegateBasedLocator(Func<IType, string, object> locatorDelegate)
		{
			if (locatorDelegate == null) { throw new ArgumentNullException("locatorDelegate"); }

			this.locatorDelegate = locatorDelegate;
		}

		protected override object Locate(IType type, string id)
		{
			return locatorDelegate(type, id);
		}
	}
}
