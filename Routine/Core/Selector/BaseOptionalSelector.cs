using System;
using System.Collections.Generic;

namespace Routine.Core.Selector
{
	public abstract class BaseOptionalSelector<TConcrete, TFrom, TItem> : IOptionalSelector<TFrom, TItem>
		where TConcrete : BaseOptionalSelector<TConcrete, TFrom, TItem>
	{
		private Func<TFrom, bool> whenDelegate;

		protected BaseOptionalSelector()
		{
			When(t => true);
		}

		public TConcrete When(Func<TFrom, bool> whenDelegate) { this.whenDelegate = whenDelegate; return (TConcrete)this;}

		protected virtual bool CanSelect(TFrom obj)
		{
			return whenDelegate(obj);
		}

		private List<TItem> SafeSelect(TFrom obj)
		{
			if(!CanSelect(obj)) { throw new CannotSelectException(obj); }

			return Select(obj);
		}

		private bool TrySelect(TFrom obj, out List<TItem> result)
		{
			if(!CanSelect(obj))
			{
				result = null;
				return false;
			}

			result = Select(obj);
			return true;
		}

		protected abstract List<TItem> Select(TFrom obj);

		#region IOptionalSelector implementation
		bool IOptionalSelector<TFrom, TItem>.CanSelect(TFrom obj)
		{
			return CanSelect(obj);
		}

		bool IOptionalSelector<TFrom, TItem>.TrySelect(TFrom obj, out List<TItem> result)
		{
			return TrySelect(obj, out result);
		}
		#endregion

		#region ISelector implementation
		List<TItem> ISelector<TFrom, TItem>.Select(TFrom obj)
		{
			return SafeSelect(obj);
		}
		#endregion
	}
}
