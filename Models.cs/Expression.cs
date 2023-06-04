using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Models.cs
{
    public class Expression
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    public static class Expression_Extn
    {
        private static readonly string[] logicalOperators = new string[] { "and", "or" };
        private static bool HasOperator(string token) => logicalOperators.Contains(token.ToLower());

        public static (string Left, string Right) GetExpressionChildrenNames(this Expression expression)
        {
            //get rid of spaces
            var splitTokens = expression.Value?.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            //check that there are 3 tokens one of which is an operator
            if (splitTokens == null || splitTokens.Count() != 3 || splitTokens.Count(x => HasOperator(x)) != 1)
                throw new ArgumentException($"Expression Value is not in the correct format: 'A and B', 'C or D'", nameof(expression.Value));

            var names = splitTokens.Where(token => !HasOperator(token));
            var left = names.First();
            var right = names.Last();
            if (names.First() == names.Last())
                throw new ArgumentException($"Expression is duplicated '{expression.Name} : {expression.Value}'", nameof(expression));
            return (left, right);
        }

        public static Rule? GetRule(this Expression expr, IEnumerable<Rule> rules)
            => rules.FirstOrDefault(rule => expr.Value.Split(logicalOperators, StringSplitOptions.None).Select(x => x?.Trim()).Any(split => split == null ? false : split.IndexOf(rule.Name, StringComparison.InvariantCultureIgnoreCase) > -1));

        public static string? ResolveExpressions(this IEnumerable<Expression>? expressions, IEnumerable<Rule>? rules)
        {
            if (expressions == null || rules == null || !expressions.Any() || !rules.Any())
                return "_";



            throw new NotImplementedException();
        }
    }
}
