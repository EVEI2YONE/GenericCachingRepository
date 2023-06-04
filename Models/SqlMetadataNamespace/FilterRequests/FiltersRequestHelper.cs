using Models.FilterExpressionNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.SqlMetadataNamespace
{
    public partial class FiltersRequest
    {
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
            => $"{Join(GroupBy)}:{Join(SortBy)}:{OrderSequence}:{ExpressionHash}";
        private string Join(IEnumerable<string>? list)
            => list == null ? "_" : string.Join(",", list);
    }
}
