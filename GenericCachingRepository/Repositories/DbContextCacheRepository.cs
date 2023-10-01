using GenericCachingRepository.Helpers;
using GenericCachingRepository.SharedCache;
using Microsoft.EntityFrameworkCore;

namespace GenericCachingRepository.Repositories
{
    public interface IDbContextCacheRepository
    {
        public Task<T?> FindAsync<T>(params object[] ids) where T : class;
        public Task AddAsync<T>(T item) where T : class;
        public Task DeleteAsync<T>(params object[] ids) where T : class;
        public Task UpdateAsync<T>(T item) where T : class;
        public Task UpsertAsync<T>(T item) where T : class;
    }

    public class DbContextCacheRepository : IDbContextCacheRepository
    {
        private readonly ICache _cache;
        private readonly DbContext _dbContext;
        public DbContextCacheRepository(DbContext dbContext, ICache? cache = null)
        {
            _dbContext = dbContext;
            _cache = cache ?? new Cache();
        }

        private object?[]? GetIds<T>(T? item) where T : class
            => CacheKeyHelper.GetIds(item);
        private string? GetKeyIds<T>(params object?[]? ids) where T : class
            => CacheKeyHelper.GetKeyIds<T>(ids);
        private string? GetKey<T>(T? item) where T : class
            => CacheKeyHelper.GetKey(item);

        public async Task AddAsync<T>(T item) where T : class
        {
            await _dbContext.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            _cache.Add(GetKey(item), item);
        }

        public async Task DeleteAsync<T>(params object[] ids) where T : class
        {
            var itemToRemove = await _dbContext.FindAsync<T>(ids);
            if (itemToRemove != null)
            {
                _dbContext.Remove(itemToRemove);
                await _dbContext.SaveChangesAsync();
            }
            _cache.Remove<T>(GetKeyIds<T>(ids));
        }
        public async Task DeleteAsync<T>(T item) where T : class
        {
            await DeleteAsync<T>(CacheKeyHelper.GetIds(item));
        }

        public async Task<T?> FindAsync<T>(params object?[]? ids) where T : class
        {
            var fetchedItem = _cache.Get<T>(key: GetKeyIds<T>(ids));
            if (fetchedItem == null)
            {
                fetchedItem = await _dbContext.FindAsync<T>(ids);
                _cache.Add(GetKey(fetchedItem), fetchedItem);
            }
            return fetchedItem;
        }

        public async Task UpdateAsync<T>(T? item) where T : class
        {
            if (item == null)
                return;
            var itemToUpdate = await _dbContext.FindAsync<T>(GetIds(item));
            if (itemToUpdate != null)
            {
                _dbContext.Entry(itemToUpdate).State = EntityState.Detached;
                _dbContext.Update(item);
                await _dbContext.SaveChangesAsync();
                _cache.Remove<T>(GetKey(itemToUpdate));
                _cache.Add(GetKey(item), item);
            }
            else
                _cache.Remove<T>(GetKey(item));
        }

        public async Task UpsertAsync<T>(T? item) where T : class
        {
            if (item == null)
                return;
            var fetchedItem = await FindAsync<T>(GetIds(item));
            if (fetchedItem == null)
                await AddAsync(item);
            else
                await UpdateAsync(fetchedItem);
            _cache.Add(GetKey(item), item);
        }
    }
}
