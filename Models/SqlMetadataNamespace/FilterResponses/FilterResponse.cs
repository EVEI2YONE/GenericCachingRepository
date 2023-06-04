using Models.EnumNamespace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.SqlMetadataNamespace.FilterResponses
{
    public partial class FilterResponse
    {
        public string? Order { get; set; }
        public string OrderBy { get; set; }

        private string DefaultEnum(string? enumName = null)
            => EnumHelper.MapEnumOrDefault<OrderBy>(enumName, EnumNamespace.OrderBy.Asc);
        private void IsEmptyList() { }
    }
}
