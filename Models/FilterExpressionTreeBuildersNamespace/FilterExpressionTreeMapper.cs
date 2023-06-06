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
            => FilterExpressionMetadata.GetUnderlyingExpression(expressionName) != expressionName;

        public string? NormalizeExpressionName(string expressionName)
            => FilterExpressionMetadata.GetUnderlyingExpression(expressionName);

        public string GetAlias(string alias)
            => _aliases[alias];

        public string? MapExpression(FilterExpression? expression)
        {
            if (expression == null)
                return null;
            var (left, op, right) = ((FilterExpressionMetadata)expression).ExtractTokensFromSyntax();
            return $"{_aliases[left]} {op} {_aliases[right]}";
        }

        public string MapAliasCorrelation(FilterExpressionMetadata expr1, FilterExpressionMetadata expr2)
        {
            return $"{BeforeAndAfter(expr1)}, {BeforeAndAfter(expr2)}";
        }

        private string BeforeAndAfter(FilterExpressionMetadata expression)
        {
            var before = expression.Value;
            var (left, _, right) = expression.ExtractTokensFromSyntax();
            var mappings = string.Join(", ", new List<string>() { left, right }
                .Where(x => _aliases.IsMapped(x))
                .Select(x => $"{{{x} = {_aliases[x]}}}")
            );
            var mappedMessage = mappings.Any() ? $", {mappings}" : string.Empty;
            var after = MapExpression(expression.Value);
            return $"{expression.Name}: {before} => {after}{mappedMessage}";
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
