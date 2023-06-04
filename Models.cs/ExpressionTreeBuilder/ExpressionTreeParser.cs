using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.cs.ExpressionTreeBuilder
{
    public partial class ExpressionTreeBuilder
    {
        private bool parentSet;
        public bool TryFindNode(ExpressionTree? expression, string? name, out ExpressionTree? node)
        {
            parentSet = false;
            node = FindNode(expression, name);
            return node != null;
        }


        public ExpressionTree? FindNode(ExpressionTree? expressionTree, string? name)
        {
            if (HasNoValue(expressionTree?.Expression?.Name, name))
                return null;
            else if (NamesMatch(expressionTree.Expression.Name, name))
                return expressionTree;

            //establish relationship between parent and found child node
            var leftChild = FindNode(expressionTree.Left, name);
            var rightChild = FindNode(expressionTree.Right, name);

            if (!parentSet && !HasNoValue(leftChild, rightChild))
            {
                if (leftChild != null)
                {
                    leftChild.Parent = expressionTree;
                    expressionTree.Left = leftChild;
                }
                else
                {
                    rightChild.Parent = expressionTree;
                    expressionTree.Right = rightChild;
                }
                parentSet = true;
            }

            return leftChild ?? rightChild;
        }
    }
}
