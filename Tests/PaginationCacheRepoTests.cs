using DbFirstTestProject.DataLayer.Context;
using GenericCachingRepository.SharedCache;
using GenericCachingRepository.SourceCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class PaginationCacheRepoTests
    {
        private EntityProjectContext context;
        private PaginationCacheRepository repo;

        [OneTimeSetUp] 
        public void OneTimeSetUp()
        {
            context = new EntityProjectContext();
        }

        [SetUp]
        public void SetUp()
        {
            var cache = new QueryCache();
            repo = new PaginationCacheRepository(context, cache);
        }

        [Test]
        public void Pagination_Test()
        {
            
        }
    }
}
