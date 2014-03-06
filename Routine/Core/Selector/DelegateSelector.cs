using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Selector
{
	public class DelegateSelector<TFrom, TItem> : BaseOptionalSelector<DelegateSelector<TFrom, TItem>, TFrom, TItem>
	{
		private readonly Func<TFrom, IEnumerable<TItem>> itemListDelegate;
		private Dictionary<TFrom, List<TItem>> cache;

		public DelegateSelector(Func<TFrom, IEnumerable<TItem>> itemListDelegate)
		{
			this.itemListDelegate = itemListDelegate;
		}

		public DelegateSelector<TFrom, TItem> UseCache() 
		{
			cache = new Dictionary<TFrom, List<TItem>>();
			return this;
		}

		protected override List<TItem> Select(TFrom obj)
		{
			if(cache == null) {return itemListDelegate(obj).ToList();}

			List<TItem> result;
			if(!cache.TryGetValue(obj, out result))
			{
				lock(cache)
				{
					if(!cache.TryGetValue(obj, out result))
					{
						result = itemListDelegate(obj).ToList();
						cache[obj] = result;
					}
				}
			}

			return result;
		}
	}
}
