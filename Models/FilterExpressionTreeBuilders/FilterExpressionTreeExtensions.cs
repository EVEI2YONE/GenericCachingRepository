using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreeBuilders
{
    public partial class FilterExpressionTreeBuilder
    {
        public static bool NamesMatch(string? name, string? nameToCompare)
            => HasNoValue(name, nameToCompare)
            ? false
            : string.Compare(name, nameToCompare, StringComparison.InvariantCultureIgnoreCase) == 0;

        public static bool HasNoValue(params object[] inputs)
            => inputs.Any(input => string.IsNullOrEmpty(input?.ToString()));
    }
}
