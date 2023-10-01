using GenericCachingRepository.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace GenericCachingRepository.Models
{
    public interface IQuery
    {
        public PagedResult<T>? Evaluate<T>(DbSet<T> dbSet) where T : class;
    }

    public class Query : IQuery
    {
        public IEnumerable<Order>? Order { get; set; }
        public IQueryableWhere? Where { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int RowCount { get; set; } = 500;

        public PagedResult<T> Evaluate<T>(DbSet<T> dbSet) where T : class
        {
            IQueryable<T>? query = (Where == null)
                ? dbSet.AsQueryable()
                : Where.EvaulateWhere(dbSet);
            var order = GetOrderAsString<T>();
            query = query.OrderBy(order);
            return query.PageResult<T>(Page, PageSize, RowCount);
        }

        private string GetOrderAsString<T>() where T : class
        {
            string order;
            if (Order?.Any() ?? false)
                order = string.Join(", ", Order.Select(o => o.Evaluate()));
            else
                order = string.Join(", ", PaginationHelper.GetKeyPropertyNames<T>()) + $" {SortOrder.Asc.ToString()}";
            return order;
        }

        public IQueryable<T> EvaulateOrder<T>(DbSet<T> dbSet, IQueryable<T>? query = null) where T : class
        {
            var order = GetOrderAsString<T>();
            if (query == null)
                return dbSet.OrderBy(order);
            query = query.OrderBy(order);
            return query;
        }
    }
}
