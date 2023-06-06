using Models.EnumNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionNamespace
{
    public class FilterExpression
    {
        public static implicit operator FilterExpression?(string? rule)
        {
            rule = rule?.Trim();
            if (string.IsNullOrWhiteSpace(rule))
                return null;
            var ruleSections = rule.Split(':');

            var expression = new FilterExpression() { Name = ruleSections.First()?.Trim(), Value = ruleSections.Last()?.Trim() };
            try { ((FilterExpressionMetadata)expression).GetExpressionChildrenNames(); }
            catch (Exception) { return null; }
            return expression;
        }

        public string? Name { get; set; }
        public string? Value { get; set; }

        public override string ToString()
            => $"{Name}: {Value}";
    }
}
