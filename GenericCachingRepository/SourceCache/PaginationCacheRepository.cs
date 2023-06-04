using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCachingRepository.SharedCache;
using Models.cs;

namespace GenericCachingRepository.SourceCache
{
    public class PaginationCacheRepository
    {
        ICache _cache;
        public PaginationCacheRepository(ICache cache)
        {
            _cache = cache;
        }
    }
}
