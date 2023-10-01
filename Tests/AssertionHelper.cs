using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Tests.Fixtures;

namespace Tests
{
    internal static class AssertionHelper
    {
        public static void AssertSame(object clause, object expect) => Assert.That(clause.ToString(), Is.EqualTo(expect.ToString()));
        public static void AssertAtLeastOne<T>(List<T> list, string? clause = null, string? order = null) where T : class
        {
            var res = list.AsQueryable();
            if(clause != null)
                res = res.Where(clause);
            if(order != null)
                res = res.OrderBy(order);
            Assert.That(res.Any(), Is.True);
        }
        public static void AssertListOrder(List<TestWhereClass> reorderedListDynamic, List<TestWhereClass> reorderedListLINQ)
        {
            for (int i = 0; i < reorderedListDynamic.Count; i++)
            {
                Assert.That(reorderedListDynamic[i], Is.EqualTo(reorderedListLINQ[i]));
            }
        }
    }
}
