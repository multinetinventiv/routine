using Microsoft.Extensions.Caching.Memory;

namespace Routine.Core.Cache
{
    public class WebCache : ICache
    {
        // remove memory cache, use dictionary instead
        private readonly MemoryCache cache;

        public WebCache()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public object this[string key] => cache.TryGetValue(key, out var result) ? result : default;
        public bool Contains(string key) => cache.TryGetValue(key, out _);
        public void Add(string key, object value) => cache.Set(key, value);
        public void Remove(string key) => cache.Remove(key);
    }
}

