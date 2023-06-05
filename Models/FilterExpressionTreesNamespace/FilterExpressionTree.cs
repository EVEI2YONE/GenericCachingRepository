using Models.FilterExpressionNamespace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreesNamespace
{
    [DebuggerDisplay("{Expression?.Name ?? \"No Expression\", nq} : {Expression?.Value ?? \"Placeholder\", nq}")]
    public partial class FilterExpressionTree
    {
        public FilterExpressionTree? Parent { get; set; }
        public FilterExpressionTree? Left { get; set; }
        public FilterExpressionTree? Right { get; set; }
        public FilterExpressionMetadata? Expression { get; set; }
    }
}
