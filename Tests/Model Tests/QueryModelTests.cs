using GenericCachingRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
using GenericCachingRepository;
using Tests.Fixtures;

namespace Tests.Model_Tests
{
    internal class QueryModelTests
    {
        private void AssertSame(object clause, object expect) => Assert.That(clause.ToString(), Is.EqualTo(expect.ToString()));
        private void AssertAtLeastOne(string clause)
        {
            var res = list.AsQueryable().Where(clause);
            Assert.That(res.Any(), Is.True);
        }
        private void AssertListOrder(List<TestWhereClass> reorderedListDynamic, List<TestWhereClass> reorderedListLINQ)
        {
            for (int i = 0; i < reorderedListDynamic.Count; i++)
            {
                Assert.That(reorderedListDynamic[i], Is.EqualTo(reorderedListLINQ[i]));
            }
        }

        List<TestWhereClass> list = new List<TestWhereClass>();

        [OneTimeSetUp] 
        public void SetUp()
        {
            list.AddRange(new List<TestWhereClass>()
            {
                new TestWhereClass()
                {
                    Col1 = "Alice",
                    Col2 = 12,
                    Col3 = 1.0f,
                    Col4 = Convert.ToDateTime("9/28/2020 02:00:01 AM"),
                    Col5 = false,
                },
                new TestWhereClass()
                {
                    Col1 = "Bonnie",
                    Col2 = 23,
                    Col3 = 2.25f,
                    Col4 = Convert.ToDateTime("9/29/2021 02:00:00 AM"),
                    Col5 = true,
                },
                new TestWhereClass()
                {
                    Col1 = "Clyde",
                    Col2 = 34,
                    Col3 = 3.75f,
                    Col4 = Convert.ToDateTime("9/29/2022 12:00:00 PM"),
                    Col5 = false,
                },
                new TestWhereClass()
                {
                    Col1 = "Derek",
                    Col2 = 45,
                    Col3 = null,
                    Col4 = Convert.ToDateTime("9/30/2023 02:00:00 AM"),
                    Col5 = true,
                }
            });
        }

        [Test]
        public void SortOrder_Test()
        {
            var order = new Order() { Column = "Col1", SortOrder = SortOrder.Asc };
            var clause = order.ToString();
            Assert.That(clause, Is.EqualTo("Col1 Asc"));

            var asc = list.AsQueryable().OrderBy(clause).ToList();
            var _list = list.OrderBy(o => o.Col1).ToList();
            AssertListOrder(asc, _list);

            order = new Order() { Column = "Col1", SortOrder = SortOrder.Desc };
            clause = order.ToString();
            Assert.That(clause, Is.EqualTo("Col1 Desc"));

            asc = list.AsQueryable().OrderBy(clause).ToList();
            _list = list.OrderByDescending(o => o.Col1).ToList();
            AssertListOrder(asc, _list);
        }

        [Test]
        public void WhereValue_Null_Simple()
        {
            var where = new WhereValue()
            {
                Column = "Col3",
                Value = null,
                Operation = ComparativeOperation.Equal
            };

            var clause = where.Evaluate<TestWhereClass>();
            //var expect = @"Col3 is null";
            var expect = @"Col3 == null";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereValue_String_Simple()
        {
            var where = new WhereValue()
            {
                Column = "Col1",
                Operation = ComparativeOperation.Like,
                Value = "C"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col1.ToString().Contains(""C"")";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereValue_Int_Simple()
        {
            var where = new WhereValue()
            {
                Column = "Col2",
                Operation = ComparativeOperation.Like,
                Value = "2"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col2.ToString().Contains(""2"")";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereValue_Float_Simple()
        {
            var where = new WhereValue()
            {
                Column = "Col3",
                Operation = ComparativeOperation.Like,
                Value = "3"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col3.ToString().Contains(""3"")";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereValue_DateTime_Simple()
        {
            var where = new WhereValue()
            {
                Column = "Col4",
                Operation = ComparativeOperation.Like,
                Value = "2023"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col4.ToString().Contains(""2023"")";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereValue_Boolean_Simple()
        {
            var where = new WhereValue()
            {
                Column = "Col5",
                Operation = ComparativeOperation.Like,
                Value = "True"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col5.ToString().Contains(""True"")";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);

            where = new WhereValue()
            {
                Column = "Col5",
                Operation = ComparativeOperation.Equal,
                Value = "True"
            };

            clause = where.Evaluate<TestWhereClass>();
            expect = @"Col5 == True";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereRange_String_Simple()
        {
            var where = new WhereRange()
            {
                Column = "Col1",
                Operation = RangeOperation.Between,
                Start = "B",
                End = "C"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col1 >= ""B"" and Col1 < ""C""";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereRange_Int_Simple()
        {
            var where = new WhereRange()
            {
                Column = "Col2",
                Operation = RangeOperation.Between,
                Start = "1",
                End = "15"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col2 >= 1 and Col2 < 15";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereRange_Float_Simple()
        {
            var where = new WhereRange()
            {
                Column = "Col3",
                Operation = RangeOperation.Between,
                Start = "1.2",
                End = "5.8"
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @"Col3 >= 1.2 and Col3 < 5.8";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }

        [Test]
        public void WhereRange_DateTime_Simple()
        {
            var now = list.First(o => o.Col1 == "Alice").Col4;
            var yesterday = now.AddSeconds(-1);
            var tomorrow = now.AddSeconds(1);

            var where = new WhereRange()
            {
                Column = "Col4",
                Operation = RangeOperation.Between,
                Start = yesterday.ToString(),
                End = tomorrow.ToString()
            };

            var clause = where.Evaluate<TestWhereClass>();
            var expect = @$"Col4 >= DateTime({yesterday.DateTimeLINQFormat()}) and Col4 < DateTime({tomorrow.DateTimeLINQFormat()})";
            AssertSame(clause, expect);
            AssertAtLeastOne(clause);
        }
    }
}
