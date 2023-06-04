using GenericCachingRepository.SharedCache;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

namespace Tests
{
    public class CacheTests
    {
        Cache cache;
        string key = "entry";
        [SetUp]
        public void Setup()
        {
            cache = new Cache();
        }

        [Test]
        public void CacheEntry()
        {
            var list1 = new List<int>();
            var list2 = new List<string>();
            var list3 = new List<object>();

            cache.Add(key, list1);

            Assert.IsNotNull(cache.Get<List<int>>(key));
            Assert.IsNull(cache.Get<List<string>>(key));
            Assert.IsNull(cache.Get<List<object>>(key));
        }

        [Test]
        public async Task CacheEntry_Dictionary()
        {
            var list1 = new List<int>();
            var _cache = new Cache(2, 2, 1);
            var tempkey = key + "temp";

            _cache.Add(key, list1, true);
            _cache.Add(tempkey, list1);

            Assert.IsNotNull(_cache.Get<List<int>>(key));
            Assert.IsNotNull(_cache.Get<List<int>>(tempkey));
            await Task.Delay(2000);
            Assert.IsNotNull(_cache.Get<List<int>>(key));
            Assert.That(_cache.Get<List<int>>(key).Count(), Is.EqualTo(0));
            Assert.IsNull(_cache.Get<List<int>>(tempkey));
        }
    }
}