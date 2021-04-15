using Microsoft.Extensions.Caching.Memory;

namespace Routine.Core.Cache
{
    public class WebCache : ICache
    {
        private readonly MemoryCache Cache;

        public WebCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
        public bool Contains(string key)
        {
            var dummyVariable = default(object);
            return Cache.TryGetValue(key, out dummyVariable);
        }

        public void Add(string key, object value)
        {
            Cache.Set(key, value);
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public object this[string key]
        {
            get
            {
                var result = default(object);
                return Cache.TryGetValue(key, out result) ? result : default;
            }
        }
    }
}

