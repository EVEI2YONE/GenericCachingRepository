using GenericCachingRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.Helpers
{
    //public static class QueryBuilder
    //{
    //    private static string GetName<T>(Expression<Func<T, object>> exp) where T : class
    //    {
    //        MemberExpression body = exp.Body as MemberExpression;

    //        if (body == null)
    //        {
    //            UnaryExpression ubody = (UnaryExpression)exp.Body;
    //            body = ubody.Operand as MemberExpression;
    //        }

    //        return body.Member.Name;
    //    }

    //    public static Tuple<List<Where>, string> WhereBy<T>(Expression<Func<T, object>> expressionName) where T : class
    //    {
    //        return WhereBy_Common(expressionName);
    //    }

    //    public static Tuple<List<Where>, string> WhereBy_Common<T>(this WhereGroup whereBy, Expression<Func<T, object>> expressionName) where T : class
    //    {
    //        return WhereBy_Common(expressionName);
    //    }

    //    private static Tuple<List<Where>, string> WhereBy_Common<T>(this WhereGroup whereBy, Expression<Func<T, object>> expressionName) where T : class
    //    {
    //        var specificClause = new List<Where>();
    //        var name = GetName<T>(expressionName);
    //        return new Tuple<List<Where>, string>(specificClause, name);
    //    }

    //    public static Tuple<List<Where>, string> OrWhereBy<T>(this WhereGroup whereBy, Expression<Func<T, object>> expressionName) where T : class
    //    {
    //        var 
    //    }

    //    public static Tuple<List<Where>, string> And<T>(this Tuple<List<Where>, string> whereBy, Expression<Func<T, object>> expressionName) where T : class
    //    {
    //        var name = GetName<T>(expressionName);
    //        return new Tuple<List<Where>, string>(whereBy.Item1, name);
    //    }

    //    public static WhereGroup Build(this Tuple<List<Where>, string> whereBy)
    //    {
    //        WhereGroup group = new WhereGroup();
    //        group.SpecificClauses = whereBy.Item1;
    //        group.OtherClauses = new List<WhereGroup>();
    //        return group;
    //    }

    //    public static Query OrderBy(this Query query, string column, bool ascending = true)
    //    {
    //        if(query != null)
    //        {
    //            query.Order = query.Order ?? new List<Order>();
    //            var list = query.Order.ToList();
    //            list.Add(new Order()
    //            {
    //                Column = column,
    //                SortOrder = ascending ? SortOrder.Asc : SortOrder.Desc
    //            });
    //            query.Order = list;
    //        }
    //        return query;
    //    }
    //}



    //public static class QueryExpressionBuilder
    //{
    //    public static Tuple<List<Where>, string> IsNull(this Tuple<List<Where>, string> whereBy)
    //    {
    //        whereBy.Item1.Add(new WhereValue()
    //        {
    //            Column = whereBy.Item2,
    //            Operation = ComparativeOperation.Equal,
    //            Value = null
    //        });
    //        return whereBy;
    //    }

    //    public static Tuple<List<Where>, string> IsLessThan(this Tuple<List<Where>, string> whereBy, object? value)
    //    {
    //        whereBy.Item1.Add(new WhereValue()
    //        {
    //            Column = whereBy.Item2,
    //            Operation = ComparativeOperation.LessThan,
    //            Value = value?.ToString()
    //        });
    //        return whereBy;
    //    }

    //    public static Tuple<List<Where>, string> IsLessThanOrEqual(this Tuple<List<Where>, string> whereBy, object? value)
    //    {
    //        whereBy.Item1.Add(new WhereValue()
    //        {
    //            Column = whereBy.Item2,
    //            Operation = ComparativeOperation.LessThanEqual,
    //            Value = value?.ToString()
    //        });
    //        return whereBy;
    //    }

    //    public static Tuple<List<Where>, string> IsGreaterThan(this Tuple<List<Where>, string> whereBy, object? value)
    //    {
    //        whereBy.Item1.Add(new WhereValue()
    //        {
    //            Column = whereBy.Item2,
    //            Operation = ComparativeOperation.GreaterThan,
    //            Value = value?.ToString()
    //        });
    //        return whereBy;
    //    }

    //    public static Tuple<List<Where>, string> IsGreaterThanOrEqual(this Tuple<List<Where>, string> whereBy, object? value)
    //    {
    //        whereBy.Item1.Add(new WhereValue()
    //        {
    //            Column = whereBy.Item2,
    //            Operation = ComparativeOperation.GreaterThanEqual,
    //            Value = value?.ToString()
    //        });
    //        return whereBy;
    //    }
    //}
}
