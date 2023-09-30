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
                return dbSet.Where($"{GetOperation<T>()}");
            query = query.Where($"{GetOperation<T>()}");
            return query;
        }

        private HashSet<string> Numbers = new HashSet<string>()
        {
            typeof(long).ToString(),
            typeof(int).ToString(),
            typeof(Int32).ToString(),
            typeof(Int16).ToString(),
            typeof(Int64).ToString(),
            typeof(float).ToString(),
            typeof(double).ToString(),
            typeof(decimal).ToString(),
        };

        public object? GetValue<T>(string column, string? value, bool containsOp = false) where T : class
        {
            if(value == null)
                return null;
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(prop => prop.Name.ToLower() == column.ToLower());
            var propertyType = propertyInfo.GetUnderlyingPropertyType();

            if (propertyType == typeof(string))
                return $@"""{value}""";
            else if (Numbers.Contains(propertyType.ToString()))
            {
                var op = value.ToString();
                return containsOp ? @$"""{op}""" : op;
            }
            else if (propertyType == typeof(DateTime))
            {
                if (containsOp)
                    return @$"""{value}""";
                var op = $"DateTime({Convert.ToDateTime(value).DateTimeLINQFormat()})";
                return op;
            }
            else if(propertyType == typeof(bool))
            {
                var op = value.ToString();
                if (containsOp)
                    return $@"""{op}""";
                return op;
            }

            return null;
        }

        public abstract string GetOperation<T>() where T : class;
        public string Evaluate<T>() where T : class => GetOperation<T>();
    }


    public class WhereValue : Where
    {
        public string Column { get; set; }
        public string? Value { get; set; }
        public ComparativeOperation Operation { get; set; }

        public override string GetOperation<T>() where T : class
        {
            switch(Operation)
            {
                case ComparativeOperation.Like: return $"{Column}.ToString().Contains({GetValue<T>(Column, Value, true)})";
                case ComparativeOperation.NotLike: return $"!{Column}.ToString().Contains({GetValue<T>(Column, Value, true)})";
                case ComparativeOperation.Equal: return $"{Column} == {GetValue<T>(Column, Value)}";
                case ComparativeOperation.NotEqual: return $"{Column} != {GetValue<T>(Column, Value)}";
                case ComparativeOperation.GreaterThan: return $"{Column} > {GetValue<T>(Column, Value)}";
                case ComparativeOperation.NotGreaterThan: return $"{Column} <= {GetValue<T>(Column, Value)}";
                case ComparativeOperation.GreaterThanEqual: return $"{Column} >= {GetValue<T>(Column, Value)}";
                case ComparativeOperation.NotGreaterThanEqual: return $"{Column} < {GetValue<T>(Column, Value)}";
                case ComparativeOperation.LessThan: return $"{Column} < {GetValue<T>(Column, Value)}";
                case ComparativeOperation.NotLessThan: return $"{Column} >= {GetValue<T>(Column, Value)}";
                case ComparativeOperation.LessThanEqual: return $"{Column} <= {GetValue<T>(Column, Value)}";
                case ComparativeOperation.NotLessThanEqual: return $"{Column} > {GetValue<T>(Column, Value)}";
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

        public override string GetOperation<T>() where T : class
        {
            switch (Operation)
            {
                case RangeOperation.Between: return $"{Column} >= {GetValue<T>(Column, Start)} and {Column} < {GetValue<T>(Column, End)}";
                case RangeOperation.NotBetween: return $"{Column} < {GetValue<T>(Column, Start)} or {Column} > {GetValue<T>(Column, End)}";
                default: throw new NotImplementedException();
            }
        }
    }
}
