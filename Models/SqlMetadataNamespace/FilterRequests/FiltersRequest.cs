using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Models.EnumNamespace;
using Models.FilterExpressionNamespace;
using Models.FilterRuleNamespace;

namespace Models.SqlMetadataNamespace
{
    public partial class FiltersRequest
    {
        public string? OrderSequence;
        public IEnumerable<string>?  SortBy { get; set; }
        public IEnumerable<string>? GroupBy { get; set; }
        public IEnumerable<FilterExpression>? Expressions { get; set; }
        public IEnumerable<FilterRule>? Rules { get; set; }
    }
}