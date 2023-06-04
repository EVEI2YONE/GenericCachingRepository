using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.cs
{
    [DebuggerDisplay("{Expression?.Name ?? \"null\"} : {Expression?.Value ?? \"null\"}")]
    public class ExpressionTree
    {
        public ExpressionTree? Parent { get; set; }
        public ExpressionTree? Left { get; set; }
        public ExpressionTree? Right { get; set; }
        public Expression? Expression { get; set; }

        public static int TotalChildren(ExpressionTree tree)
        {
            if (tree == null
                || string.IsNullOrWhiteSpace(tree.Left?.Expression?.Name)
                || string.IsNullOrWhiteSpace(tree.Right?.Expression?.Name))
                return 0;
            return TotalChildren(tree.Left) + TotalChildren(tree.Right);
        }
    }
}
