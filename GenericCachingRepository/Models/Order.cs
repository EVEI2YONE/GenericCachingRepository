namespace GenericCachingRepository.Models
{
    public class Order
    {
        public SortOrder SortOrder { get; set; } = SortOrder.Asc;
        public string Column { get; set; }
        public string Evaluate() => this.ToString();
        public override string ToString() => $"{Column} {SortOrder}";
    }
}
