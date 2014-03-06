using System;
using System.Collections.Generic;

namespace Routine.Core.Cache
{
	public class DictionaryCache : ICache
	{
		private readonly Dictionary<string, object> dictionary;

		public DictionaryCache()
		{
			dictionary = new Dictionary<string, object>();
		}

		public bool Contains(string key)
		{
			return dictionary.ContainsKey(key);
		}

		public void Add(string key, object value)
		{
			if(Contains(key))
			{
				dictionary[key] = value;
				return;
			}

			dictionary.Add(key, value);
		}

		public void Remove(string key)
		{
			if(Contains(key))
			{
				dictionary.Remove(key);
			}
		}
		public object this[string key]
		{
			get
			{
				if(!Contains(key)) {return null;}

				return dictionary[key];
			}
		}
	}
}

