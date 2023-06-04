using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.FilterExpressions;

namespace Models
{
    [DebuggerDisplay("{Expression?.Name ?? \"null\"} : {Expression?.Value ?? \"null\"}")]
    public class FilterExpressionTree
    {
        public FilterExpressionTree? Parent { get; set; }
        public FilterExpressionTree? Left { get; set; }
        public FilterExpressionTree? Right { get; set; }
        public FilterExpression? Expression { get; set; }

        public static int TotalChildren(FilterExpressionTree tree)
        {
            if (tree == null
                || string.IsNullOrWhiteSpace(tree.Left?.Expression?.Name)
                || string.IsNullOrWhiteSpace(tree.Right?.Expression?.Name))
                return 0;
            return TotalChildren(tree.Left) + TotalChildren(tree.Right);
        }
    }
}
