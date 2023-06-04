using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Models.FilterExpressions;

namespace Models
{
    public class Filters
    {
        //order will descide precedence for OrderBy and GroupBy
        public string Order { get; set; } //asc, desc
        public IEnumerable<string> OrderBy { get; set; }
        public IEnumerable<string> GroupBy { get; set; }
        public IEnumerable<FilterExpression> Expressions { get; set; }
        public IEnumerable<Rule> Rules { get; set; }

        private string? _hashcode = null;
        public string ExpressionHash
        {
            get
            {
                _hashcode = _hashcode ?? Expressions?.ResolveExpressions(Rules)?.GetHashCode().ToString() ?? "_";
                return _hashcode;
            }
        }

        public override string ToString()
            => $"{Join(OrderBy)}:{Join(GroupBy)}:{ExpressionHash}";
        private string Join(IEnumerable<string> list)
            => list == null ? "_" : string.Join(",", list);
    }
}