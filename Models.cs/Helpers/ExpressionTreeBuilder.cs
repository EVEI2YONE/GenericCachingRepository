using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Models.cs.Helpers
{
    public class ExpressionTreeBuilder
    {
        private List<ExpressionTree> _roots = new List<ExpressionTree>();
        public IEnumerable<ExpressionTree> Roots { get { return _roots; } }

        private bool NamesMatch(string? name, string? nameToCompare)
            => HasNoValue(name, nameToCompare)
            ? false
            : string.Compare(name, nameToCompare, StringComparison.InvariantCultureIgnoreCase) == 0;

        private bool HasNoValue(params object[] inputs)
            => inputs.Any(input => string.IsNullOrEmpty(input?.ToString()));

        private ExpressionTree CreateNode([Required] string name)
        =>
        new ExpressionTree()
        {
            Expression = new Expression()
            {
                Name = name,
                Value = null
            }
        };

        public void Add(Expression expression)
        {
            if (expression == null)
                return;

            var (leftName, rightName) = expression.GetExpressionChildrenNames();
            var name = expression.Name;

            if (HasNoValue(name))
                throw new ArgumentException("Expression Name expected but was null or empty", nameof(expression.Name));

            //if node was found, then root has value
            ExpressionTree? node = null;
            var root = _roots.FirstOrDefault(root => TryFindNode(root, name, out node));

            if (node != null && node.Expression?.Value != null)
                throw new ArgumentException($"Expression '{name}' already has exists.");

            //if root is null, then node is null. Assign node to root after creation
            if (root == null) //Disjoint Tree
            {
                root = CreateNode(name);
                node = root;
                _roots.Add(root);
            }


            node.Expression = expression;
            //create placeholders to build a single disjoint tree
            node.Left = CreateNode(leftName);
            node.Right = CreateNode(rightName);


            TryConnectTrees(node, node.Left);
            TryConnectTrees(node, node.Right);

            SetParent(node, node.Left);
            SetParent(node, node.Right);
        }

        private bool TryConnectTrees(ExpressionTree parent, ExpressionTree childNode)
        {
            if (HasNoValue(childNode?.Expression?.Name))
                return false;
            //just compare _roots
            var root = _roots.FirstOrDefault(root => NamesMatch(childNode.Expression.Name, root?.Expression?.Name));

            if (root != null)
            {
                //root is ChildNode
                var isLeftChild = NamesMatch(parent.Left.Expression.Name, childNode.Left.Expression.Name);
                //childNode is only name placeholder - update with Root of disjoint tree
                if (isLeftChild)
                    parent.Left = root;
                else
                    parent.Right = root;
                root.Parent = parent;
                _roots.Remove(root);
                return true;
            }
            return false;
        }

        private bool parentSet;
        private bool TryFindNode(ExpressionTree? expression, string? name, out ExpressionTree? node)
        {
            parentSet = false;
            node = FindNode(expression, name);
            return node != null;
        }

        private void SetParent(ExpressionTree parent, ExpressionTree? child)
        {
            if (child != null)
                child.Parent = parent;
        }

        private ExpressionTree? FindNode(ExpressionTree? expressionTree, string? name)
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