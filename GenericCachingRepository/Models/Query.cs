namespace GenericCachingRepository.Models
{
    public class Query
    {
        public IEnumerable<Order> Order { get; set; }
        public IEnumerable<IQueryableWhere> Where { get; set; }
        public int Page { get; set; } = 1;
        public int Count { get; set; } = 50;
    }
}
