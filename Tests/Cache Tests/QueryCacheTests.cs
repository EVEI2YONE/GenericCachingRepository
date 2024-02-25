using DbFirstTestProject.DataLayer.Context;
using DbFirstTestProject.DataLayer.Entities;
using GenericCachingRepository.Models;
using GenericCachingRepository.SharedCache;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Tests.Fixtures;

namespace Tests.Cache_Tests
{
    internal class QueryCacheTests
    {
        private EntityProjectContext _context;
        private Random _random;
        private IQueryCache _cache;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _random = new Random();
            _context= new EntityProjectContext();

            for(int i = 0; i < 10;)
            {
                if (_context.Table1.Count() > 10)
                    break;
                _context.Add(new Table1()
                {
                    Col2 = Guid.NewGuid().ToString(),
                    Col3 = _random.Next(),
                    Col4 = Guid.NewGuid().ToString(),
                });
            }
            _context.SaveChanges();
        }

        [SetUp]
        public void Setup()
        {
            _cache = new QueryCache();
        }

        [TestCase("6")]
        public void QueryCount_Test(string value)
        {
            var partial_pk = new WhereValue() {
                Column = nameof(Table1.Col1_PK),
                Value = value,
                Operation = ComparativeOperation.Like
            };

            var clause = partial_pk.Evaluate<Table1>();
            var list = _context.Table1.Where(clause);
            var count = list.Count();

            Assert.True(list.Any());

            _cache.SetQueryCount<Table1>(clause, count);
            Assert.That(count, Is.EqualTo(_cache.GetQueryCount<Table1>(clause))); //cached successfully
            Assert.That(_cache.GetQueryCount<Table2>(clause) , Is.EqualTo(0)); //cached only by type
        }

        [TestCase("2")]
        public async Task QueryLoad_Save_Test(string value)
        {
            var partial_pk = new WhereValue()
            {
                Column = nameof(Table1.Col1_PK),
                Value = value,
                Operation = ComparativeOperation.Like
            };
            var orderBy = new Order()
            {
                Column = nameof(Table1.Col1_PK),
                SortOrder = SortOrder.Desc
            }.Evaluate();

            var clause = partial_pk.Evaluate<Table1>();
            var list = _context.Table1.Where(clause).OrderBy(orderBy).ToList();
            var count = list.Count();

            Assert.True(list.Any());

            var block = 0;
            var queryKey = $"{orderBy}:{block}:{clause}";
            _cache.SetQueryCount<Table1>(clause, count);
            _cache.SaveCacheQueryReferences(queryKey, list);

            var _cacheList = (await _cache.LoadCacheQueryReferences(_context.Table1, queryKey)).ToList();

            for (int i = 0; i < count; i++)
                Assert.That(list[i], Is.EqualTo(_cacheList[i]));
        }
    }
}
