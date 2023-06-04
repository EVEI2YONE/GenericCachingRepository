﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Models.EnumNamespace;
using Models.RuleNamespace;

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

        private bool HasOperator(string token) => EnumHelper.MapEnum<LogicalOperator>(token) != null;

        public (string Left, string Right) GetExpressionChildrenNames()
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

            Value = $"{left} {EnumHelper.MapEnum<LogicalOperator>(splitTokens.ElementAt(1)).ToLower()} {right}";

            return (left, right);
        }

        public static bool NamesMatch(string? name, string? nameToCompare)
            => HasNoValue(name, nameToCompare)
            ? false
            : string.Compare(GetUnderlyingExpression(name), GetUnderlyingExpression(nameToCompare), StringComparison.InvariantCultureIgnoreCase) == 0;

        public static bool HasNoValue(params object[] inputs)
            => inputs.Any(input => string.IsNullOrEmpty(input?.ToString()));

        private static string? GetUnderlyingExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;
            return expression.Trim().Replace("!", "");
        }
        private static  void ValidateNotExpression(string expression)
        {
            var underlyingExpresion = GetUnderlyingExpression(expression);
            if (expression.Length - underlyingExpresion?.Length > 1 || string.IsNullOrEmpty(underlyingExpresion))
                throw new ArgumentException($"Expression has invalid number of NOT operators: '{expression}'");
        }
    }
}
