using Models.FilterRuleNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionNamespace
{
    public static class FilterExpressionResolver
    {
        public static string? ResolveExpressions(this IEnumerable<FilterExpression>? expressions, IEnumerable<FilterRule>? rules)
        {
            if (expressions == null || rules == null || !expressions.Any() || !rules.Any())
                return "_";

            throw new NotImplementedException();
        }
    }
}
