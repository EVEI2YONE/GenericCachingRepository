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

        public string MapExpression(FilterExpression expression)
        {
            var (left, op, right) = expression.ExtractTokensFromSyntax();
            return $"{_aliases[left]} {op} {_aliases[right]}";
        }

        public (string Left, string Right) NormalizeAndRegisterAliases(string leftName, string rightName)
        {
            var (_left, _right) = (NormalizeExpressionName(leftName), NormalizeExpressionName(rightName));
            _aliases.TryAddAlias(_left, _left);
            _aliases.TryAddAlias(_right, _right);
            return (_aliases[_left], _aliases[_right]);
        }
    }
}
