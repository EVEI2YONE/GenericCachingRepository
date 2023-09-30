using DbFirstTestProject.DataLayer.Context;
using DbFirstTestProject.DataLayer.Entities;
using GenericCachingRepository.SourceCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class dbContextCacheRepoTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task Cache_CRUD(bool deleteItemInstance)
        {
            var item = new Table1() { Col2 = "Insert" };
            var context = new EntityProjectContext();
            var repo = new DbContextCacheRepository(context);

            await repo.AddAsync(item);

            var updatedItem = new Table1() { Col1_PK = item.Col1_PK, Col2 = "Update" };
            await repo.UpdateAsync(updatedItem);

            var keys = CacheKeyHelper.GetIds(updatedItem);
            var cacheItem = await repo.FindAsync<Table1>(keys);
            Assert.IsNotNull(cacheItem);
            Assert.That(cacheItem.Col2, Is.EqualTo(updatedItem.Col2));

            if (deleteItemInstance)
                await repo.DeleteAsync(cacheItem);
            else
                await repo.DeleteAsync<Table1>(keys);
            cacheItem = await repo.FindAsync<Table1>(keys);
            Assert.IsNull(cacheItem);
        }
    }
}
