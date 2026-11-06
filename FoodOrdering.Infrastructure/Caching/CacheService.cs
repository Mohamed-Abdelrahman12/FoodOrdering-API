using Microsoft.Extensions.Caching.Memory;

namespace FoodOrdering.Infrastructure.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(15);

        public CacheService(IMemoryCache cache) => _cache = cache;

        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry
            };
            _cache.Set(key, value, options);
        }

        public void Remove(string key) => _cache.Remove(key);

        public bool TryGet<T>(string key, out T? value)
            => _cache.TryGetValue(key, out value);
    }
}
