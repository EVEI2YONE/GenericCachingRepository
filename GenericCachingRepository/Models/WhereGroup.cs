using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.Models
{
    public interface IQueryableWhere
    {
        public IQueryable GetSpecificClause<T>(DbSet<T> dbSet, IQueryable? query = null) where T : class;
        public IQueryable GetOtherClause<T>(DbSet<T> dbSet, IQueryable? query = null) where T : class;
    }

    public class WhereGroup : IQueryableWhere
    {
        public IEnumerable<WhereGroup>? OtherClauses { get; set; } // (_) OR (_) OR (_)
        public IEnumerable<Where>? SpecificClauses { get; set; } // OR (_ AND _ AND _) OR

        public string GetSpecificClauseAsString()
        {
            if(SpecificClauses?.Any() ?? false)
            {
                return $"({string.Join(" and ", SpecificClauses)})";
            }
            return string.Empty;
        }

        public override string ToString()
        {
            OtherClauses = OtherClauses ?? new List<WhereGroup>().AsEnumerable();
            var clause = GetSpecificClauseAsString();
            var otherGroups = string.Join(" or ", OtherClauses
                .Select(c => $"({c?.ToString() ?? string.Empty})")
                .Where(c => c.Length > 2)
            );
            var or = !string.IsNullOrWhiteSpace(clause) ? " or " : string.Empty;
            if (!string.IsNullOrWhiteSpace(otherGroups))
                clause += $"{or}{otherGroups}";
            return clause;
        }

        public IQueryable GetSpecificClause<T>(DbSet<T> dbSet, IQueryable? query = null) where T : class
        {
            var clause = this.ToString();
            if (query == null)
                return dbSet.Where(clause);
            query = query.Where(clause);
            return query;
        }

        public IQueryable GetOtherClause<T>(DbSet<T> dbSet, IQueryable? query = null) where T : class
        {
            var clause = this.ToString();
            if (query == null)
                return dbSet.Where(clause);
            query = query.Where(clause);
            return query;
        }
    }
}
