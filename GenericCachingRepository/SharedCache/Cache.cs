using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Primitives;
using System.ComponentModel;

namespace GenericCachingRepository.SharedCache
{
    public interface ICache
    {
        public void Add<T>(string? key, T? item, bool dictionaryEntry = false) where T : class;
        public T? Remove<T>(string? key) where T : class;
        public T? Get<T>(string? key) where T : class;
    }

    public class Cache : ICache
    {
        protected readonly IMemoryCache cache;
        private readonly MemoryCacheEntryOptions memoryCacheEntryOptions;
        private readonly MemoryCacheEntryOptions dictionaryEntryOptions = new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove };
        private readonly MemoryCacheOptions cacheOptions;
        public Cache(int absoluteExpirationSeconds = 300, int slidingExpirationSeconds = 60, int expirationFrequencyScanSeconds = 1)
        {
            cacheOptions = new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromSeconds(expirationFrequencyScanSeconds),
            };

            memoryCacheEntryOptions = memoryCacheEntryOptions ?? new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, 0, absoluteExpirationSeconds, 0),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromSeconds(slidingExpirationSeconds),
            };

            cache = new MemoryCache(cacheOptions);
        }
        public void Add<T>(string? key, T? item, bool dictionaryEntry = false) where T : class
        {
            if (string.IsNullOrWhiteSpace(key) || item == null)
                return;
            cache.CreateEntry(key);
            cache.Set(key, item, dictionaryEntry ? dictionaryEntryOptions : memoryCacheEntryOptions);
        }

        public T? Remove<T>(string? key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var item = Get<T>(key);
            cache.Remove(key);
            return item;
        }

        public T? Get<T>(string? key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            cache.TryGetValue<T>(key, out var item);
            return item;
        }
    }
}