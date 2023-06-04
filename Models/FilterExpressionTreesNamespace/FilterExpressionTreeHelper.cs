using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreesNamespace
{
    public partial class FilterExpressionTree
    {
        public static int TotalChildren(FilterExpressionTree? tree)
        {
            if (tree == null)
                return 0;
            else if (tree.Left == null && tree.Right == null)
                return 1;
            return TotalChildren(tree.Left) + TotalChildren(tree.Right) + 1;
        }
    }
}
