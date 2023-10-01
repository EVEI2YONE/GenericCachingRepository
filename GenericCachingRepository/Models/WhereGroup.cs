using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace GenericCachingRepository.Models
{
    public interface IQueryableWhere
    {
        public IQueryable<T> EvaulateWhere<T>(DbSet<T> dbSet, IQueryable<T>? query = null) where T : class;
    }

    public class WhereGroup : IQueryableWhere
    {
        public IEnumerable<WhereGroup>? OtherClauses { get; set; } // (_) OR (_) OR (_)
        public IEnumerable<Where>? SpecificClauses { get; set; } // OR (_ AND _ AND _) OR

        public string GetSpecificClauseAsString<T>() where T : class
        {
            if (SpecificClauses?.Any() ?? false)
            {
                return $"({string.Join(" and ", SpecificClauses.Select(o => o.Evaluate<T>()))})";
            }
            return string.Empty;
        }

        private string GetWhereAsString<T>(IEnumerable<WhereGroup> groupClauses) where T : class
        {
            groupClauses = groupClauses ?? new List<WhereGroup>().AsEnumerable();
            var clause = GetSpecificClauseAsString<T>();
            var otherGroups = string.Join(" or ", groupClauses
                .Select(c => $"({c?.ToString() ?? string.Empty})")
                .Where(c => c.Length > 2)
            );
            var or = !string.IsNullOrWhiteSpace(clause) ? " or " : string.Empty;
            if (!string.IsNullOrWhiteSpace(otherGroups))
                clause += $"{or}{otherGroups}";
            return clause;
        }

        public IQueryable<T> EvaulateWhere<T>(DbSet<T> dbSet, IQueryable<T>? query = null) where T : class
        {
            var clause = GetWhereAsString<T>(OtherClauses);
            var isEmpty = string.IsNullOrWhiteSpace(clause);
            if (query == null)
            {
                return isEmpty ? dbSet.AsQueryable() : dbSet.Where(clause);
            }
            query = isEmpty ? query : query.Where(clause);
            return query;
        }
    }
}
