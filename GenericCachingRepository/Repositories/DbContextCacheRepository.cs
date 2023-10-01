using GenericCachingRepository.Helpers;
using GenericCachingRepository.SharedCache;
using Microsoft.EntityFrameworkCore;

namespace GenericCachingRepository.Repositories
{
    public interface IDbContextCacheRepository
    {
        public Task<T?> FindAsync<T>(params object[] ids) where T : class;
        public Task<bool> AddAsync<T>(T item) where T : class;
        public Task<bool> DeleteAsync<T>(params object[] ids) where T : class;
        public Task<bool> DeleteAsync<T>(T item) where T : class;
        public Task<T?> UpdateAsync<T>(T item) where T : class;
        public Task<T> UpsertAsync<T>(T item) where T : class;
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

        public async Task<bool> AddAsync<T>(T item) where T : class
        {
            try
            {
                await _dbContext.AddAsync(item);
                await _dbContext.SaveChangesAsync();
                _cache.Add(GetKey(item), item);
                return true;
            }
            catch(Exception)
            { 
                return false;
            }
        }

        public async Task<bool> DeleteAsync<T>(params object[] ids) where T : class
        {
            try
            {
                bool removed = false;
                var itemToRemove = await _dbContext.FindAsync<T>(ids);
                if (itemToRemove != null)
                {
                    _dbContext.Remove(itemToRemove);
                    await _dbContext.SaveChangesAsync();
                    removed = true;
                }
                _cache.Remove<T>(GetKeyIds<T>(ids));
                return removed;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteAsync<T>(T item) where T : class
        {
            return await DeleteAsync<T>(CacheKeyHelper.GetIds(item));
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

        public async Task<T?> UpdateAsync<T>(T? item) where T : class
        {
            if (item == null)
                return null;
            try
            {
                var itemToUpdate = await _dbContext.FindAsync<T>(GetIds(item));
                if (itemToUpdate != null)
                {
                    _dbContext.Entry(itemToUpdate).State = EntityState.Detached;
                    _dbContext.Update(item);
                    await _dbContext.SaveChangesAsync();
                    _cache.Remove<T>(GetKey(itemToUpdate));
                    _cache.Add(GetKey(item), item);
                    return item;
                }
                else
                    _cache.Remove<T>(GetKey(item));
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<T> UpsertAsync<T>(T? item) where T : class
        {
            if (item == null)
                return null;
            try
            {
                var fetchedItem = await FindAsync<T>(GetIds(item));
                if (fetchedItem == null)
                    await AddAsync(item);
                else
                    await UpdateAsync(fetchedItem);
                _cache.Add(GetKey(item), item);
                return item;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
