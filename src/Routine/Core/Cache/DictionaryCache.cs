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

        public object this[string key] => Contains(key) ? dictionary[key] : null;
        public bool Contains(string key) => dictionary.ContainsKey(key);

        public void Add(string key, object value)
        {
            if (Contains(key))
            {
                dictionary[key] = value;
                return;
            }

            dictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            if (Contains(key))
            {
                dictionary.Remove(key);
            }
        }
    }
}

