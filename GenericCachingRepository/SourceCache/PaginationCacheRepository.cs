using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericCachingRepository.SharedCache;

namespace GenericCachingRepository.SourceCache
{
    public class PaginationCacheRepository
    {

        ICache _cache;
        private IDictionary<Type, object> locks = new Dictionary<Type, object>();
        public PaginationCacheRepository(ICache cache)
        {
            _cache = cache;
        }


    }
}
