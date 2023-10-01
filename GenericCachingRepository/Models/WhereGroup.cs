using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace GenericCachingRepository.Models
{
    public interface IQueryableWhere
    {
        public IQueryable<T> EvaulateWhere<T>(DbSet<T> dbSet, IQueryable<T>? query = null) where T : class;
        public string GetClauseAsString<T>() where T : class;
    }

    public class WhereGroup : IQueryableWhere
    {
        public IEnumerable<WhereGroup>? OtherClauses { get; set; } // (_) OR (_) OR (_)
        public IEnumerable<Where>? SpecificClauses { get; set; } // OR (_ AND _ AND _) OR

        public string GetSpecificClauseAsString<T>(IEnumerable<Where>? specificClause) where T : class
        {
            var clause = string.Empty;
            if (specificClause?.Any() ?? false)
            {
                clause = $"({string.Join(" and ", specificClause.Select(o => o.Evaluate<T>()))})";
            }
            return clause;
        }

        private string GetWhereAsString<T>(IEnumerable<WhereGroup>? groupClauses) where T : class
        {
            if (groupClauses == null)
                return string.Empty;

            var clause = string.Join(" or ", groupClauses
                .Select(c => $"{GetClause_Recursive<T>(c.OtherClauses, c.SpecificClauses)}")
                .Where(c => c.Length > 2)
            );
            return clause;
        }

        private string GetClause_Recursive<T>(IEnumerable<WhereGroup>? otherClauses, IEnumerable<Where>? specificClauses) where T : class
        {
            var specific = GetSpecificClauseAsString<T>(specificClauses);
            var otherClause = GetWhereAsString<T>(otherClauses);
            var hasSpecific = !string.IsNullOrWhiteSpace(specific);
            var hasOther = !string.IsNullOrWhiteSpace(otherClause);
            var or = (hasSpecific && hasOther) ? " or " : string.Empty;
            var clause = $"{specific}{or}{otherClause}";
            return clause;
        }

        public string GetClauseAsString<T>() where T : class
            => GetClause_Recursive<T>(OtherClauses, SpecificClauses);

        public IQueryable<T> EvaulateWhere<T>(DbSet<T> dbSet, IQueryable<T>? query = null) where T : class
        {
            var clause = GetClauseAsString<T>();
            var useClause = !string.IsNullOrWhiteSpace(clause);
            if (query == null)
            {
                return useClause ? dbSet.Where(clause) : dbSet.AsQueryable();
            }
            query = useClause ? query : query.Where(clause);
            return query;
        }
    }
}
