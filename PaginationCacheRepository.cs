using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository
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
