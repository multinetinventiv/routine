using System.Collections.Generic;
using System.Linq;
using System;

namespace Routine.Core.Reflection
{
	internal static class MemberIndex
	{
		public static MemberIndex<TKey, TItem> Build<TKey, TItem>(IEnumerable<TItem> list, Func<TItem, TKey> keyFunction)
		{
			var result = new MemberIndex<TKey, TItem>();

			foreach(var item in list)
			{
				result.Add(keyFunction(item), item);
			}

			return result;
		}
	}

	internal class MemberIndex<TKey, TItem>
	{
		private readonly Dictionary<TKey, List<TItem>> index;

		public MemberIndex()
		{
			index = new Dictionary<TKey, List<TItem>>();
		}

		public void Add(TKey key, TItem item)
		{
			if(!index.ContainsKey(key))
			{
				index.Add(key, new List<TItem>());
			}

			index[key].Add(item);
		}

		public TItem GetFirstOrDefault(TKey key)
		{
			List<TItem> result;

			if(!index.TryGetValue(key, out result))
			{
				return default(TItem);
			}

			if(result.Count <= 0)
			{
				return default(TItem);
			}

			return result[0];
		}

		public List<TItem> GetAll(TKey key)
		{
			List<TItem> result;

			if(!index.TryGetValue(key, out result))
			{
				return new List<TItem>();
			}

			return result;
		}
	}
}

