using System;

namespace Routine.Core.Locator
{
	public class DelegateLocator : BaseOptionalLocator<DelegateLocator>
	{
		private readonly Func<TypeInfo, string, object> locatorDelegate;

		public DelegateLocator(Func<TypeInfo, string, object> locatorDelegate)
		{
			this.locatorDelegate = locatorDelegate;
		}

		protected override object Locate(TypeInfo type, string id)
		{
			return locatorDelegate(type, id);
		}
	}
}
