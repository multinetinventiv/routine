using System;
using System.Collections.Generic;
using Routine.Core.Selector;

namespace Routine.Core.Builder
{
	public class SelectorBuilder<TFrom, TItem>
	{
		public DelegateSelector<TFrom, TItem> By(Func<TFrom, IEnumerable<TItem>> itemListDelegate)
		{
			return new DelegateSelector<TFrom, TItem>(itemListDelegate);
		}

		public StaticSelector<TFrom, TItem> Always(params TItem[] staticResult)
		{
			return new StaticSelector<TFrom, TItem>(staticResult);
		}

		public NoneSelector<TFrom, TItem> None()
		{
			return new NoneSelector<TFrom, TItem>();
		}
	}
}
