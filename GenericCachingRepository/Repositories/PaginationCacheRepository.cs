﻿using GenericCachingRepository.SharedCache;
using Microsoft.EntityFrameworkCore;

namespace GenericCachingRepository.Repositories
{
    public class PaginationCacheRepository
    {
        private DbContext _context;
        private IQueryCache _cache;
        private IDictionary<Type, object> locks = new Dictionary<Type, object>();

        public PaginationCacheRepository(DbContext context, IQueryCache cache)
        {
            _cache = cache;
            _context = context;
        }


    }
}
