using Models.DictionaryNamespace;
using Models.EnumNamespace;
using Models.FilterExpressionNamespace;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreeAnalyzerNamespace
{
    public partial class FilterExpressionTreeAnalyzer
    {
        /*
        1: A or B
        2: C or B //A or B | 2 => 1 (Index expression with their tokens, and then map expressions to find duplicates)

        A: 3 or 4
        C: 3 or 4 //C => A
        */

        /*
        1: A or B
        1: C or B //1 is already defined
        A: 2 or 3
        C: 2 or 3 //A = C
        //Both defined expressions of 1 are equivalent
            1                   1                       1
        A   or  B           C   or  B       =>      A/C or  B
    2   or  3          2   or  3                2   or  3

        1: A or B
        1: C or B //1 is already defined, Alias C = A
        A: 2 or 3 //Register C: 2 to 3
        C: 4 or 3 //A = C, Alias 2 = 4
        3: A or B //3 = 1, but A: 2 or 3 is invalid. Cannot contain itself in a defintion
        */

        private IDictionary<string, List<FilterExpression>> alreadyDefinedExpressions = new Dictionary<string, List<FilterExpression>>();
        private IDictionary<string, IEnumerable<FilterExpression>> duplicateExpressions;


        private AliasDictionary aliases = new AliasDictionary();
        private IDictionary<string, IEnumerable<FilterExpressionWrapper>> mappedExpressions = new Dictionary<string, IEnumerable<FilterExpressionWrapper>>();
        private IDictionary<string, List<FilterExpression>> registeredExpressions = new Dictionary<string, List<FilterExpression>>();
        private IEnumerable<FilterExpression> NormalizeExpressions(IEnumerable<FilterExpression> expressions)
        {
            //Validate and Normalize syntax of expressions
            IEnumerable<FilterExpressionMetadata> filterExpressions = 
                new List<FilterExpressionMetadata>(expressions.Select(x => (FilterExpressionMetadata)x));

            List<FilterExpressionMetadata> explicitDuplicates = new List<FilterExpressionMetadata>();
            //Register expression names
            foreach (var expr in filterExpressions)
            {
                Try(tryAction: () => 
                    {
                        CreateList(expr.Name);
                        aliases[expr.Name] = expr.Name;
                    }, 
                    tryCatch: () => 
                    { 
                        if (aliases[expr.Name] == expr.Name 
                        && registeredExpressions.ContainsKey(expr.Name)
                        && registeredExpressions[expr.Name] != expr)
                        {

                        } 
                    },
                    tryFinally: () => registeredExpressions[expr.Name].Add(expr));
            }


            return null;
        }

        private FilterExpressionWrapper ValidateSyntax(FilterExpression expression)
        {
            var (left, op, right) = ((FilterExpressionMetadata)expression).ExtractTokensFromSyntax();
            return new FilterExpressionWrapper()
            {
                OriginalExpression = $"{expression.Name}: {expression.Value}",
                OriginalValue = expression.Value,
                Name = expression.Name,
                Left = left,
                Right = right,
                Operator = op,
            };
        }

        private void CreateList(string name)
        {
            if(!registeredExpressions.ContainsKey(name))
                registeredExpressions[name] = new List<FilterExpression>();
        }

        private void Try(Action tryAction, Action? tryCatch = null, Action? tryFinally = null)
        { 
            try { tryAction(); } 
            catch (Exception) { if(tryCatch != null) tryCatch(); } 
            finally { if(tryFinally != null) tryFinally(); } 
        }
    }

    internal class FilterExpressionWrapper
    {
        public string OriginalExpression { get; set; }
        public string OriginalValue { get; set; }
        public string Name { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public LogicalOperator Operator { get; set; }
    }
}
