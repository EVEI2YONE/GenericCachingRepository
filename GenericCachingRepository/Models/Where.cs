using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace GenericCachingRepository.Models
{
    public abstract class Where
    {
        public IQueryable GetClause<T>(DbSet<T> dbSet, IQueryable? query = null) where T : class
        {
            if (query == null)
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

        private string Quote(object? value) => $@"""{value?.ToString()}""";

        public object? GetValue<T>(string column, string? value, bool encloseInQuotes, params Type[] typeExceptions) where T : class
        {
            string? op = "null";
            if (value != null)
            {
                var propertyInfo = typeof(T).GetProperties().FirstOrDefault(prop => prop.Name.ToLower() == column.ToLower());
                var propertyType = propertyInfo.GetUnderlyingPropertyType();

                if (encloseInQuotes)
                    op = Quote(value);
                else if (propertyType == typeof(string))
                    op = value;
                else if (Numbers.Contains(propertyType.ToString()))
                    op = value.ToString();
                else if (propertyType == typeof(DateTime))
                    op = $"DateTime({Convert.ToDateTime(value).DateTimeLINQFormat()})";
                else if (propertyType == typeof(bool))
                    op = value.ToString();

                if(typeExceptions?.Contains(propertyType) ?? false)
                    op = Quote(value);
            }
            return op;
        }

        public abstract string GetOperation<T>() where T : class;
        public string Evaluate<T>() where T : class => GetOperation<T>();
    }


    public class WhereValue : Where
    {
        public string Column { get; set; }
        public string? Value { get; set; }
        public ComparativeOperation Operation { get; set; }

        private string? Coalesce(params string?[] values) => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        public override string GetOperation<T>() where T : class
        {
            string value;
            string? nullStr = null;
            bool containsOp = false;

            switch (Operation)
            {
                case ComparativeOperation.Like:
                case ComparativeOperation.NotLike:
                    value = (string?)GetValue<T>(Column, Value, true) ?? string.Empty;
                    break;
                default:
                    value = (string?) GetValue<T>(Column, Value, false) ?? string.Empty;
                    break;
            }


            string response;
            switch (Operation)
            {
                case ComparativeOperation.Like:                 response = $"{Column}.ToString().Contains({value})";   break;
                case ComparativeOperation.NotLike:              response = $"!{Column}.ToString().Contains({value})";  break;
                case ComparativeOperation.Equal:                response = $"{Column} {Coalesce(nullStr, "==")} {value}";   break;
                case ComparativeOperation.NotEqual:             response = $"{Column} {Coalesce(nullStr, "!=")} {value}";   break;
                case ComparativeOperation.GreaterThan:          response = $"{Column} > {value}";  break;
                case ComparativeOperation.NotGreaterThan:       response = $"{Column} <= {value}"; break;
                case ComparativeOperation.GreaterThanEqual:     response = $"{Column} >= {value}"; break;
                case ComparativeOperation.NotGreaterThanEqual:  response = $"{Column} < {value}";  break;
                case ComparativeOperation.LessThan:             response = $"{Column} < {value}";  break;
                case ComparativeOperation.NotLessThan:          response = $"{Column} >= {value}"; break;
                case ComparativeOperation.LessThanEqual:        response = $"{Column} <= {value}"; break;
                case ComparativeOperation.NotLessThanEqual:     response = $"{Column} > {value}";  break;
                default: throw new NotImplementedException();
            }
            return response.Trim();
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
            var start = GetValue<T>(Column, Start, false, typeof(string));
            var end   = GetValue<T>(Column, End, false, typeof(string));
            switch (Operation)
            {
                case RangeOperation.Between: return $"{Column} >= {start} and {Column} < {end}";
                case RangeOperation.NotBetween: return $"{Column} < {start} or {Column} > {end}";
                default: throw new NotImplementedException();
            }
        }
    }
}
