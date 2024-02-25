//using DbFirstTestProject.DataLayer.Context;
//using DbFirstTestProject.DataLayer.Entities;
//using GenericCachingRepository.Helpers;
//using GenericCachingRepository.Models;

//namespace Tests.Model_Tests
//{
//    internal class ExtensionQueryBuilderTests
//    {
//        private EntityProjectContext _context;
//        private Query _query;
//        private string col1 = nameof(Table1.Col1_PK);
//        private string col2 = nameof(Table1.Col2);
//        private string col3 = nameof(Table1.Col3);
//        private string col4 = nameof(Table1.Col4);

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _context = new EntityProjectContext();

//            _query = new Query()
//            {
//                Page = 1,
//                PageSize = 10,
//                RowCount = 5,
//                Order = new List<Order>()
//                {
//                    new Order()
//                    {
//                        Column = col2,
//                        SortOrder = SortOrder.Asc
//                    },
//                    new Order()
//                    {
//                        Column = col3,
//                        SortOrder = SortOrder.Asc
//                    }
//                },
//                Where = new WhereGroup()
//                {
//                    SpecificClauses = new List<Where>()
//                    {
//                        new WhereValue()
//                        {
//                            Column = col1,
//                            Value = "4144917",
//                            Operation = ComparativeOperation.LessThan
//                        },
//                        new WhereValue()
//                        {
//                            Column = col4,
//                            Value = null,
//                            Operation = ComparativeOperation.Equal
//                        }
//                    },
//                    OtherClauses = new List<WhereGroup>()
//                    {
//                        new WhereGroup()
//                        {
//                            SpecificClauses = new List<Where>()
//                            {
//                                new WhereValue()
//                                {
//                                    Column = col2,
//                                    Value = "Insert",
//                                    Operation = ComparativeOperation.Equal
//                                }
//                            }
//                        },
//                        new WhereGroup()
//                        {
//                            SpecificClauses = new List<Where>()
//                            {
//                                new WhereRange()
//                                {
//                                    Column = col3,
//                                    Start = "2",
//                                    End  = "4",
//                                    Operation = RangeOperation.Between
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            var (where, order) = _query.Evaluate<Table1>();
//            var expectedWhere = @$"(Col1_PK < 4144917 and Col4 == null) or (Col2 == ""Insert"") or (Col3 >= 2 and Col3 < 4)";
//            var expectedOrder = $@"Col2 Asc, Col3 Asc";

//            AssertionHelper.AssertSame(where, expectedWhere);
//            AssertionHelper.AssertSame(order, expectedOrder);
//        }

//        [SetUp]
//        public void SetUp()
//        {

//        }

//        [Test]
//        public void Extension_Test()
//        {
//            var query = new Query()
//            {
//                Page = 1,
//                PageSize = 10,
//                RowCount = 5,
//            };

//            //var where = new WhereGroup()
//            //{
//            //    SpecificClauses = new List<Where>()
//            //    {
//            //        new WhereValue()
//            //        {
//            //            Column = col1,
//            //            Value = "4144917",
//            //            Operation = ComparativeOperation.LessThan
//            //        },
//            //        new WhereValue()
//            //        {
//            //            Column = col4,
//            //            Value = null,
//            //            Operation = ComparativeOperation.Equal
//            //        }
//            //    },
//            //    OtherClauses = new List<WhereGroup>()
//            //    {
//            //        new WhereGroup()
//            //        {
//            //            SpecificClauses = new List<Where>()
//            //            {
//            //                new WhereValue()
//            //                {
//            //                    Column = col2,
//            //                    Value = "Insert",
//            //                    Operation = ComparativeOperation.Equal
//            //                }
//            //            }
//            //        },
//            //        new WhereGroup()
//            //        {
//            //            SpecificClauses = new List<Where>()
//            //            {
//            //                new WhereRange()
//            //                {
//            //                    Column = col3,
//            //                    Start = "2",
//            //                    End  = "4",
//            //                    Operation = RangeOperation.Between
//            //                }
//            //            }
//            //        }
//            //    }
//            //};

//            //(Col1_PK < 4144917 and Col4 is null) or(Col2 = 'Insert') or(Col3 >= 2 and Col3 < 4)
//            var where = QueryBuilder.WhereBy<Table1>(o => o.Col1_PK)
//                .IsLessThanOrEqual(4144916)
//                .And<Table1>(o => o.Col4).IsNull()
//                .Build();

//            //where.OrWhereBy<Table1>(o => o.Col2)
//            //    .EqualsTo("Insert")
//            //    .OrWhereBy<Table1>(o => o.Col3)
//            //    .IsLessThan(4);

//            query.OrderBy(col1)
//                 .OrderBy(col2);

//            Assert.True(query.Order?.Any() ?? false);
//        }
//    }
//}
