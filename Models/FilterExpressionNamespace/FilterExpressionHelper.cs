using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Models.RulesNamespace;

namespace Models.FilterExpressionNamespace
{
    public partial class FilterExpression
    {
        public static implicit operator FilterExpression?(string? rule)
        {
            rule = rule?.Trim();
            if (string.IsNullOrWhiteSpace(rule))
                return null;
            var ruleSections = rule.Split(':');
            
            var expression = new FilterExpression() { Name = ruleSections.First()?.Trim(), Value = ruleSections.Last()?.Trim()};
            try { expression.GetExpressionChildrenNames(); }
            catch (Exception) { return null; }
            return expression;
        }

        private static readonly string[] logicalOperators = new string[] { "and", "or" };
        private bool HasOperator(string token) => logicalOperators.Contains(token.ToLower());

        public (string Left, string Right) GetExpressionChildrenNames()
        {
            //get rid of spaces
            var splitTokens = Value?.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            //check that there are 3 tokens one of which is an operator
            if (splitTokens == null || splitTokens.Count() != 3 || splitTokens.Count(x => HasOperator(x)) != 1)
                throw new ArgumentException($"Expression Value is not in the correct format: 'A and B', 'C or D'", nameof(Value));

            var names = splitTokens.Where(token => !HasOperator(token));
            var left = names.First();
            var right = names.Last();
            if (names.First() == names.Last())
                throw new ArgumentException($"Expression is duplicated '{Name} : {Value}'", typeof(FilterExpression).Name);
            return (left, right);
        }

        public Rule? GetRule(IEnumerable<Rule> rules)
            => rules.FirstOrDefault(rule => Value.Split(logicalOperators, StringSplitOptions.None).Select(x => x?.Trim()).Any(split => split == null ? false : split.IndexOf(rule.Name, StringComparison.InvariantCultureIgnoreCase) > -1));
    }
}
