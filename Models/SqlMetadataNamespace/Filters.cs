using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Models.FilterExpressionNamespace;
using Models.RulesNamespace;

namespace Models.SqlMetadataNamespace
{
    public partial class Filters
    {
        //order will descide precedence for OrderBy and GroupBy
        public string Order { get; set; } //asc, desc
        public IEnumerable<string> OrderBy { get; set; }
        public IEnumerable<string> GroupBy { get; set; }
        public IEnumerable<FilterExpression> Expressions { get; set; }
        public IEnumerable<Rule> Rules { get; set; }
    }
}