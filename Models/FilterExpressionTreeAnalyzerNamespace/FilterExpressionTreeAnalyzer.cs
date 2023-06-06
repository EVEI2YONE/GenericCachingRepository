using Models.FilterExpressionNamespace;
using Models.FilterExpressionTreeBuildersNamespace;
using Models.FilterRuleNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreeAnalyzerNamespace
{
    public partial class FilterExpressionTreeAnalyzer
    {
        private readonly IEnumerable<FilterExpression> expressions;
        private readonly IEnumerable<FilterRule> rules;
        private readonly FilterExpressionTreeBuilder builder;
        public FilterExpressionTreeAnalyzer(IEnumerable<FilterExpression> expressions, IEnumerable<FilterRule> rules)
        {
            this.expressions = NormalizeExpressions(expressions);
            this.rules = rules;
            this.builder = new FilterExpressionTreeBuilder();
        }

        public bool HasValidStructure() => builder.Roots.Count() == 0;
        public IEnumerable<FilterExpression?> GetCyclicalRules() => builder.Roots.Where(x => x.Parent != null).Select(x => (FilterExpression?) x.Expression);
        public IEnumerable<FilterRule> GetEmptyRules() => null;

        public async Task Build(Func<FilterExpression> ExpressionFailureCallback, Func<FilterRule> RuleFailureCallback)
        {
            await Task.Run(() =>
            {
                foreach(var expression in expressions)
                {
                    try
                    {
                        builder.Add(expression);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                foreach(var rule in rules)
                {
                    try
                    {
                        builder.Add(rule);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            });
        }
    }
}
