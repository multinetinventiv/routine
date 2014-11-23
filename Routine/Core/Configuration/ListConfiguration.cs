using System.Collections.Generic;

namespace Routine.Core.Configuration
{
	public class ListConfiguration<TConfiguration, TItem>
	{
		private readonly TConfiguration configuration;
		private readonly string name;
		private readonly List<TItem> list;

		public ListConfiguration(TConfiguration configuration, string name)
		{
			this.configuration = configuration;
			this.name = name;

			list = new List<TItem>();
		}

		public TConfiguration Add(params TItem[] items){return Add(items as IEnumerable<TItem>);}
		public TConfiguration Add(IEnumerable<TItem> items)
		{
			list.AddRange(items);

			return configuration;
		}

		public List<TItem> Get()
		{
			return list;
		}

		public TConfiguration Merge(ListConfiguration<TConfiguration, TItem> other)
		{
			list.AddRange(other.list);

			return configuration;
		}
	}
}