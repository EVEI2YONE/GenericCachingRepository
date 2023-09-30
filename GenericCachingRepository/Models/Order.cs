using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.Models
{
    public class Order
    {
        public SortOrder SortOrder { get; set; } = SortOrder.Asc;
        public string Column { get; set; }

        public override string ToString() => $"{Column} {SortOrder}";
    }
}
