using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Selector
{
	public class MultipleSelector<TConfigurator, TFrom, TItem> : ISelector<TFrom, TItem>
	{
		private readonly TConfigurator configurator;
		private readonly List<IOptionalSelector<TFrom, TItem>> selectors;
		private readonly Excluder<TConfigurator, TFrom, TItem> excluder;

		public MultipleSelector(TConfigurator configurator)
		{
			this.configurator = configurator;
			this.selectors = new List<IOptionalSelector<TFrom, TItem>>();
			this.excluder = new Excluder<TConfigurator, TFrom, TItem>(this);
		}

		public Excluder<TConfigurator, TFrom, TItem> Exclude { get { return excluder; } }
		public TConfigurator Done() { return configurator; }
		public TConfigurator Done(IOptionalSelector<TFrom, TItem> selector) { Add(selector); return configurator; }

		public MultipleSelector<TConfigurator, TFrom, TItem> Add(IOptionalSelector<TFrom, TItem> selector)
		{
			this.selectors.Add(selector);

			return this;
		}

		public MultipleSelector<TConfigurator, TFrom, TItem> Merge(MultipleSelector<TConfigurator, TFrom, TItem> other)
		{
			selectors.AddRange(other.selectors);

			excluder.Merge(other.excluder);

			return this;
		}

		protected virtual List<TItem> Select(TFrom obj)
		{
			var result = new List<TItem>();

			try
			{
				foreach(var selector in selectors)
				{
					List<TItem> curResult;
					if(selector.TrySelect(obj, out curResult))
					{
						result.AddRange(curResult);
					}
				}
			}
			catch(NoMoreItemsShouldBeSelectedException){}

			return result.Distinct().Where(ItemIsIncluded).ToList();
		}

		private bool ItemIsIncluded(TItem item)
		{
			return !(excluder as IExcluder<TItem>).ItemIsExcluded(item);
		}

		#region ISelector implementation
		List<TItem> ISelector<TFrom, TItem>.Select(TFrom type)
		{
			return Select(type);
		}
		#endregion
	}

	public class NoMoreItemsShouldBeSelectedException : Exception {}

}
