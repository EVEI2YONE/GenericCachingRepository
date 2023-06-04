using Models.FilterExpressionNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreeBuildersNamespace
{
    public partial class FilterExpressionTreeBuilder
    {
        public bool HasNot(string expressionName)
            => FilterExpression.GetUnderlyingExpression(expressionName) != expressionName;

        public string? NormalizeExpressionName(string expressionName)
            => FilterExpression.GetUnderlyingExpression(expressionName);

        public string GetAlias(string alias)
            => _aliases[alias];
    }
}
