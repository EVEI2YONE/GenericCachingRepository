using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Models.EnumNamespace;

namespace Models.FilterExpressionNamespace
{
    public partial class FilterExpressionMetadata
    {
        private bool HasOperator(string token) => EnumHelper.MapEnum<LogicalOperator>(token) != null;
        public (string Left, string Right) GetExpressionChildrenNames()
        {
            var (left, _, right) = ExtractTokensFromSyntax();
            return (left, right);
        }
        public (string Left, LogicalOperator op, string Right) ExtractTokensFromSyntax()
        {
            //get rid of spaces
            var splitTokens = Value?.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            //check that there are 3 tokens one of which is an operator
            if (splitTokens == null || splitTokens.Count() != 3 || splitTokens.Count(x => HasOperator(x)) != 1)
                throw new ArgumentException($"Expression Value is not in the correct format: '{Value}'", nameof(Value));

            var names = splitTokens.Where(token => !HasOperator(token));
            var left = names.First();
            var right = names.Last();
            if (names.First() == names.Last())
                throw new ArgumentException($"Expression is duplicated '{Name} : {Value}'", typeof(FilterExpression).Name);

            ValidateNotExpression(left);
            ValidateNotExpression(right);

            var op = EnumHelper.GetEnum<LogicalOperator>(splitTokens.ElementAt(1));
            Value = $"{left} {op} {right}";
            return (splitTokens.First(), op, splitTokens.Last());
        }
        public static bool ValuesMatch(string value1, string value2)
        {
            if (HasNoValue(value1, value2))
                return false;
            return value1 == value2;
        }
        public static bool NamesMatch(string? name, string? nameToCompare)
            => HasNoValue(name, nameToCompare)
            ? false
            : string.Compare(GetUnderlyingExpression(name), GetUnderlyingExpression(nameToCompare), StringComparison.InvariantCultureIgnoreCase) == 0;
        public static bool HasNoValue(params object?[] inputs)
            => inputs.Any(input => string.IsNullOrEmpty(input?.ToString()));
        public static string? GetUnderlyingExpression(string expression)
            => expression?.Trim()?.Replace("!", "");
        private static string ValidateNotExpression(string expression)
        {
            var underlyingExpresion = GetUnderlyingExpression(expression);
            if (expression.Length - underlyingExpresion?.Length > 1 || string.IsNullOrEmpty(underlyingExpresion))
                throw new ArgumentException($"Expression has invalid number of NOT operators: '{expression}'");
            return underlyingExpresion;
        }
    }
}
