using GenericCachingRepository.Models;
using GenericCachingRepository.SharedCache;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace GenericCachingRepository.Repositories
{
    public interface IPaginationCacheRepository
    {
        public PagedResult<T> GetPage<T>(Query? query = null) where T : class;
    }

    public class PaginationCacheRepository : IPaginationCacheRepository
    {
        private DbContext _context;
        private IQueryCache _cache;
        private IDictionary<Type, object> locks = new Dictionary<Type, object>();

        public PaginationCacheRepository(DbContext context, IQueryCache cache)
        {
            _cache = cache;
            _context = context;
        }

        public PagedResult<T> GetPage<T>(Query? query) where T : class
        {
            return query?.GetPage(_context.Set<T>()) ?? new PagedResult<T>();
        }
    }
}
