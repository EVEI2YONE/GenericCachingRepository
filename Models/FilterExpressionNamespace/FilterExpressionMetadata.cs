using Models.EnumNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionNamespace
{
    public class FilterExpressionMetadata
    {
        public FilterExpressionMetadata() { }
        public FilterExpressionMetadata(FilterExpression expression)
        {
            Name = expression.Name;
            Value = expression.Value;
            LogicalOperator = expression.ExtractTokensFromSyntax().op;
        }

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
