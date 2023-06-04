using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using Models.FilterExpressionNamespace;
using Models.FilterExpressionTreeBuildersNamespace;
using Models.FilterExpressionTreesNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.XPath;

namespace Tests
{
    internal class FilterExpressionTreeBuilderTests
    {
        FilterExpressionTreeBuilder builder;
        List<FilterExpression?> expressions;

        [SetUp]
        public void SetUp()
        {
            builder = new FilterExpressionTreeBuilder();
            expressions = new List<FilterExpression?>()
            {
                "0 : 1 or 2",
                "1: A  or B",
                "2: 3 or  D",
                "3: E or 4 ",
                " 4: V or D",
                "5: C or B",
                "6: 5 or G",
                "A: 6 or G"
            };
            Assert.That(expressions.Count(), Is.EqualTo(8));
        }

        private void AssertCount(int count) => Assert.That(builder.Roots.Count(), Is.EqualTo(count));
        private void AssertCount(FilterExpressionTree tree, int count) => Assert.That(FilterExpressionTree.TotalChildren(tree), Is.EqualTo(count));
        private void AssertFilterExpressionTreeNodeEquals(FilterExpressionTree FilterExpressionTree, string value, string name)
        {
            Assert.IsNotNull(FilterExpressionTree);
            Assert.IsNotNull(FilterExpressionTree.Expression);
            var expression = FilterExpressionTree.Expression;
            Assert.That(expression.Value, Is.EqualTo(value));
            Assert.That(expression.Name, Is.EqualTo(name));
        }
        private (FilterExpressionTree? tree1, FilterExpressionTree? tree2) VerifyPlaceholders(FilterExpressionTree? parent,  string leftName, string rightName, string? leftValue = null, string? rightValue = null, bool isRoot = false)
        {
            var left = parent.Left;
            var right = parent.Right;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            AssertFilterExpressionTreeNodeEquals(left, leftValue, leftName);
            AssertFilterExpressionTreeNodeEquals(right, rightValue, rightName);
            Assert.That(parent, Is.EqualTo(left.Parent));
            Assert.That(parent, Is.EqualTo(right.Parent));
            return (left, right);
        }

        private void ExpectAssertException(FilterExpressionTree left, FilterExpressionTree right)
        {
            Assert.Throws<AssertionException>(() => VerifyPlaceholders(left, null, null));
            Assert.Throws<AssertionException>(() => VerifyPlaceholders(right, null, null));
        }

        [Test]
        public void BuildFilterExpressionTree_PosTest()
        {
            var _0 = expressions[0];
            var _1 = expressions[1];
            var _2 = expressions[2];
            var _3 = expressions[3];
            var _4 = expressions[4];

            builder.Add(_0);
            var expr_0 = builder.Roots.First();
            Assert.IsNotNull(expr_0);
            Assert.IsNull(expr_0.Parent);

            AssertFilterExpressionTreeNodeEquals(expr_0, "1 or 2", "0");
            var (expr_01, expr_02) = VerifyPlaceholders(expr_0, "1", "2", isRoot: true);
            ExpectAssertException(expr_01, expr_02);

            builder.Add(_1);
            AssertFilterExpressionTreeNodeEquals(expr_01, "A or B", "1");
            var (expr_01A, expr_01B) = VerifyPlaceholders(expr_01, "A", "B");
            ExpectAssertException(expr_01A, expr_01B);

            builder.Add(_2);
            AssertFilterExpressionTreeNodeEquals(expr_02, "3 or D", "2");
            var (expr_023, expr_02D) = VerifyPlaceholders(expr_02, "3", "D");
            ExpectAssertException(expr_023, expr_02D);

            builder.Add(_3);
            AssertFilterExpressionTreeNodeEquals(expr_023, "E or 4", "3");
            var (expr_023E, expr_0234) = VerifyPlaceholders(expr_023, "E", "4");
            ExpectAssertException(expr_023E, expr_0234);

            builder.Add(_4);
            AssertFilterExpressionTreeNodeEquals(expr_0234, "V or D", "4");
            var (expr_02DV, expr_02DD) = VerifyPlaceholders(expr_0234, "V", "D");
            ExpectAssertException(expr_02DV, expr_02DD);
        }

        [Test]
        public void FindFilterExpressionTreeNode()
        {
            BuildFilterExpressionTree_PosTest();
            var node = builder.FindNode(builder.Roots.First(), "2");
            Assert.IsNotNull(node);
            Assert.That(node.Expression.Name, Is.EqualTo("2"));
            Assert.That(node.Expression.Value, Is.EqualTo("3 or D"));
            Assert.That(node.Parent.Expression.Name, Is.EqualTo("0"));
            Assert.That(node.Parent.Expression.Value, Is.EqualTo("1 or 2"));
        }

        [Test]
        public void CreateDisjointTrees_PosTest()
        {
            Assert.That(builder.Roots.Count(), Is.EqualTo(0));
            BuildFilterExpressionTree_PosTest();
            var _5 = expressions[5];
            var _6 = expressions[6];

            Assert.That(builder.Roots.Count(), Is.EqualTo(1));
            builder.Add(_6);
            Assert.That(builder.Roots.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ConnectDisjointTrees_PosTest()
        {
            Assert.That(builder.Roots.Count(), Is.EqualTo(0));
            BuildFilterExpressionTree_PosTest();
            var _5 = expressions[5];
            var _6 = expressions[6];
            var _A = expressions[7];

            Assert.That(builder.Roots.Count(), Is.EqualTo(1));
            builder.Add(_5);
            Assert.That(builder.Roots.Count(), Is.EqualTo(2));
            builder.Add(_6);
            Assert.That(builder.Roots.Count(), Is.EqualTo(2));
            builder.Add(_A);
            Assert.That(builder.Roots.Count(), Is.EqualTo(1));
        }

        [Test]
        public void NotOperator_PosTest()
        {
            var currentCount = expressions.Count();
            expressions.Add("V: !A or B");
            Assert.That(expressions.Count(), Is.EqualTo(currentCount+1));

            expressions.Add("V: !!C or D");
            Assert.That(expressions.Count(), Is.EqualTo(currentCount+2));
            Assert.IsNull(expressions.Last());
        }

        [Test]
        public void ConnectDisjointTrees_NullOperatorExpression_PosTest()
        {
            Assert.That(builder.Roots.Count(), Is.EqualTo(0));
            BuildFilterExpressionTree_PosTest();
            var _5 = expressions[5];
            var _6 = expressions[6];
            expressions.Remove(expressions.Last());
            expressions.Add("A: !6 or G");
            var _A = expressions[7];

            Assert.That(builder.Roots.Count(), Is.EqualTo(1));
            builder.Add(_5);
            Assert.That(builder.Roots.Count(), Is.EqualTo(2));
            builder.Add(_6);
            Assert.That(builder.Roots.Count(), Is.EqualTo(2));
            builder.Add(_A);
            Assert.That(builder.Roots.Count(), Is.EqualTo(1));
        }
    }
}
