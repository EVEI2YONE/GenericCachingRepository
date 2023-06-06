using Models.EnumNamespace;
using Models.FilterExpressionTreeBuildersNamespace;
using Models.FilterExpressionTreesNamespace;
using Models.FilterRuleNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionNamespace
{
    public partial class FilterExpressionMetadata
    {
        public static implicit operator FilterExpression?(FilterExpressionMetadata expr) => expr.expression;
        public static implicit operator FilterExpressionMetadata?(FilterExpression expr) => new FilterExpressionMetadata(expr);
        
        public static implicit operator FilterRule?(FilterExpressionMetadata rule) => rule.rule;
        public static implicit operator FilterExpressionMetadata?(FilterRule rule) => new FilterExpressionMetadata(rule);

        private readonly FilterExpression? expression;
        private readonly FilterRule? rule;
        public FilterExpressionMetadata() { }
        public FilterExpressionMetadata(FilterExpression? expression)
        {
            if (expression == null)
                return;
            Type = FilterType.Expression;
            this.expression = expression;
            Name = expression.Name;
            Value = expression.Value;
            LogicalOperator = ExtractTokensFromSyntax().op;
        }

        public FilterExpressionMetadata(FilterRule? rule)
        {
            if (rule == null)
                return;
            Type = FilterType.Rule;
            this.rule = rule;
            Name = rule.Name;
            Value = rule.Value;
        }

        public string MappedExpression { get; set; }
        public FilterType Type { get; set; }
        public LogicalOperator LogicalOperator { get; set; }
        public bool IsNot { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
        public override string ToString()
        {
            var notOp = IsNot ? "!" : string.Empty;
            return $"{notOp}{Name}: {Value}";
        }
    }
}
