using System.Collections.Generic;

namespace Routine.Core.Selector
{
	public class NoneSelector<TFrom, TItem> : BaseOptionalSelector<NoneSelector<TFrom, TItem>, TFrom, TItem>
	{
		protected override List<TItem> Select(TFrom obj)
		{
			throw new NoMoreItemsShouldBeSelectedException();
		}
	}
}

