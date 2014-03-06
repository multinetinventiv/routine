using System.Collections.Generic;

namespace Routine.Core.Selector
{
	public class StaticSelector<TFrom, TItem> : BaseOptionalSelector<StaticSelector<TFrom, TItem>, TFrom, TItem>
	{
		private readonly List<TItem> staticResult;

		public StaticSelector(IEnumerable<TItem> staticResult)
		{
			this.staticResult = new List<TItem>(staticResult);
		}

		public StaticSelector<TFrom, TItem> And(params TItem[] additionalStaticResult) { staticResult.AddRange(additionalStaticResult); return this;}

		protected override List<TItem> Select(TFrom obj)
		{
			return staticResult;
		}
	}
}

