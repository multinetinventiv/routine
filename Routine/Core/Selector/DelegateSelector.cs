using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Selector
{
	public class DelegateSelector<TFrom, TItem> : BaseOptionalSelector<DelegateSelector<TFrom, TItem>, TFrom, TItem>
	{
		private readonly Func<TFrom, IEnumerable<TItem>> itemListDelegate;

		public DelegateSelector(Func<TFrom, IEnumerable<TItem>> itemListDelegate)
		{
			this.itemListDelegate = itemListDelegate;
		}

		protected override List<TItem> Select(TFrom obj)
		{
			return itemListDelegate(obj).ToList();
		}
	}
}
