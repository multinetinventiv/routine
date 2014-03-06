using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Selector
{
	//Builder pattern kullanilirken saklamak istenilen fonksiyonlar icin olusturuldu
	public interface IExcluder<TItem>
	{
		bool ItemIsExcluded(TItem item);
	}

	public class Excluder<TConfigurator, TFrom, TItem> : IExcluder<TItem>
	{
		private readonly MultipleSelector<TConfigurator, TFrom, TItem> parent;
		private readonly List<Func<TItem, bool>> excluders;

		public Excluder(MultipleSelector<TConfigurator, TFrom, TItem> parent)
		{
			this.parent = parent;
			this.excluders = new List<Func<TItem, bool>>();
		}

		public TConfigurator Done() { return parent.Done(); }
		public TConfigurator Done(Func<TItem, bool> excluder) { Add(excluder); return Done();}

		public Excluder<TConfigurator, TFrom, TItem> Add(Func<TItem, bool> excluder)
		{
			this.excluders.Add(excluder);

			return this;
		}

		public Excluder<TConfigurator, TFrom, TItem> Merge(Excluder<TConfigurator, TFrom, TItem> other)
		{
			excluders.AddRange(other.excluders);

			return this;
		}

		protected bool ItemIsExcluded(TItem item)
		{
			return excluders.Any(e => e(item));
		}

		#region IExcluder implementation

		bool IExcluder<TItem>.ItemIsExcluded(TItem item)
		{
			return ItemIsExcluded(item);
		}

		#endregion
	}
}
