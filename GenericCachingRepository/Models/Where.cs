using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.Models
{
    public abstract class Where
    {
        public IQueryable GetClause<T>(DbSet<T> dbSet, IQueryable? query = null) where T : class
        {
            if(query == null)
                return dbSet.Where($"{GetOperation()}");
            query = query.Where($"{GetOperation()}");
            return query;
        }
        public abstract string GetOperation();
        public override string ToString() => GetOperation();
    }


    public class WhereValue : Where
    {
        public string Column { get; set; }
        public string? Value { get; set; }
        public ComparativeOperation Operation { get; set; }

        public override string GetOperation()
        {
            switch(Operation)
            {
                case ComparativeOperation.Like: return $"{Column}.Contains({Value})";
                case ComparativeOperation.NotLike: return $"!{Column}.Contains({Value})";
                case ComparativeOperation.Equal: return $"{Column} == {Value}";
                case ComparativeOperation.NotEqual: return $"{Column} != {Value}";
                case ComparativeOperation.GreaterThan: return $"{Column} > {Value}";
                case ComparativeOperation.NotGreaterThan: return $"{Column} <= {Value}";
                case ComparativeOperation.GreaterThanEqual: return $"{Column} >= {Value}";
                case ComparativeOperation.NotGreaterThanEqual: return $"{Column} < {Value}";
                case ComparativeOperation.LessThan: return $"{Column} < {Value}";
                case ComparativeOperation.NotLessThan: return $"{Column} >= {Value}";
                case ComparativeOperation.LessThanEqual: return $"{Column} <= {Value}";
                case ComparativeOperation.NotLessThanEqual: return $"{Column} > {Value}";
                default: throw new NotImplementedException();
            }
        }
    }

    public class WhereRange : Where
    {
        public string Column { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public RangeOperation Operation { get; set; }

        public override string GetOperation()
        {
            switch (Operation)
            {
                case RangeOperation.Between: return $"({Column} >= {Start} and {Column} <= {End})";
                case RangeOperation.NotBetween: return $"{Column} < {Start} or {Column} > {End}";
                default: throw new NotImplementedException();
            }
        }
    }
}
