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

        public static int TotalChildren(FilterExpressionTree? tree)
        {
            if (tree == null)
                return 0;
            else if(tree.Left == null && tree.Right == null)
                return 1;
            return TotalChildren(tree.Left) + TotalChildren(tree.Right) + 1;
        }
    }
}
