using DbFirstTestProject.DataLayer.Context;
using DbFirstTestProject.DataLayer.Entities;
using GenericCachingRepository.Models;
using GenericCachingRepository.Repositories;
using GenericCachingRepository.SharedCache;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Repository_Tests
{
    internal class PaginationCacheRepoTests
    {
        private EntityProjectContext _context;
        private PaginationCacheRepository _repo;

        [OneTimeSetUp] 
        public void OneTimeSetUp()
        {
            _context = new EntityProjectContext();
        }

        [SetUp]
        public void SetUp()
        {
            var cache = new QueryCache();
            _repo = new PaginationCacheRepository(_context, cache);
        }

        [Test]
        public void Pagination_Test()
        {
            var col1 = nameof(Table1.Col1_PK);
            var col2 = nameof(Table1.Col2);
            var col3 = nameof(Table1.Col3);
            var col4 = nameof(Table1.Col4);

            var query = new Query()
            {
                Page = 1,
                PageSize = 10,
                RowCount = 5,
                Order = new List<Order>()
                {
                    new Order()
                    {
                        Column = col1,
                        SortOrder = SortOrder.Asc
                    },
                    new Order()
                    {
                        Column = col2,
                        SortOrder = SortOrder.Asc
                    }
                },
                Where = new WhereGroup()
                {
                    SpecificClauses = new List<Where>()
                    {
                        new WhereValue()
                        {
                            Column = col1,
                            Value = "4144917",
                            Operation = ComparativeOperation.LessThan
                        },
                        new WhereValue()
                        {
                            Column = col4,
                            Value = null,
                            Operation = ComparativeOperation.Equal
                        }
                    },
                    OtherClauses = new List<WhereGroup>()
                    {
                        new WhereGroup()
                        {
                            SpecificClauses = new List<Where>()
                            {
                                new WhereValue()
                                {
                                    Column = col2,
                                    Value = "Insert",
                                    Operation = ComparativeOperation.Equal
                                }
                            }
                        },
                        new WhereGroup()
                        {
                            SpecificClauses = new List<Where>()
                            {
                                new WhereRange()
                                {
                                    Column = col3,
                                    Start = "2",
                                    End  = "4",
                                    Operation = RangeOperation.Between
                                }
                            }
                        }
                    }
                }
            };

            var (where, order) = query.Evaluate<Table1>();
            var expectedWhere = @$"(Col1_PK < 4144917 and Col4 == null) or (Col2 == ""Insert"") or (Col3 >= 2 and Col3 < 4)";
            var expectedOrder = $@"Col1_PK Asc, Col2 Asc";

            AssertionHelper.AssertSame(where, expectedWhere);
            AssertionHelper.AssertSame(order, expectedOrder);

            var result = _repo.GetPage<Table1>(query);
            var list = result.Queryable.ToList();
            AssertionHelper.AssertAtLeastOne(list, where, order);
        }
    }
}
