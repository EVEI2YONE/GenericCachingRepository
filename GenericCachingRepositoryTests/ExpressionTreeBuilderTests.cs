using Microsoft.Extensions.DependencyInjection;
using Models.cs;
using Models.cs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace GenericCachingRepositoryTests
{
    internal class ExpressionTreeBuilderTests
    {
        ExpressionTreeBuilder builder;
        List<Expression> expressions;

        [SetUp]
        public void SetUp()
        {
            builder = new ExpressionTreeBuilder();
            expressions = new List<Expression>()
            {
                new Expression() { Name = "0", Value = "1 or 2" },
                new Expression() { Name = "1", Value = "A or B" },
                new Expression() { Name = "2", Value = "3 or D" },
                new Expression() { Name = "3", Value = "E or 4" },
                new Expression() { Name = "4", Value = "V or D" },
            };
        }

        private void AssertCount(int count) => Assert.That(builder.Roots.Count(), Is.EqualTo(count));
        private void AssertCount(ExpressionTree tree, int count) => Assert.That(ExpressionTree.TotalChildren(tree), Is.EqualTo(count));
        private void AssertExpressionTreeNodeEquals(ExpressionTree expressionTree, string value, string name)
        {
            Assert.IsNotNull(expressionTree);
            Assert.IsNotNull(expressionTree.Expression);
            var expression = expressionTree.Expression;
            Assert.That(expression.Value, Is.EqualTo(value));
            Assert.That(expression.Name, Is.EqualTo(name));
        }
        private (ExpressionTree? tree1, ExpressionTree? tree2) VerifyPlaceholders(ExpressionTree? parent,  string leftName, string rightName, string? leftValue = null, string? rightValue = null, bool isRoot = false)
        {
            var left = parent.Left;
            var right = parent.Right;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            AssertExpressionTreeNodeEquals(left, leftValue, leftName);
            AssertExpressionTreeNodeEquals(right, rightValue, rightName);
            Assert.That(parent, Is.EqualTo(left.Parent));
            Assert.That(parent, Is.EqualTo(right.Parent));
            return (left, right);
        }

        [Test]
        public void CountChildren_PosTest()
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

            AssertExpressionTreeNodeEquals(expr_0, "1 or 2", "0");
            var (expr_01, expr_02) = VerifyPlaceholders(expr_0, "1", "2", isRoot: true);

            builder.Add(_1);
            AssertExpressionTreeNodeEquals(expr_01, "A or B", "1");
            var (expr_01A, expr_01B) = VerifyPlaceholders(expr_01, "A", "B");

            builder.Add(_2);
            AssertExpressionTreeNodeEquals(expr_02, "3 or D", "2");
            var (expr_023, expr_02D) = VerifyPlaceholders(expr_02, "3", "D");

            builder.Add(_3);
            AssertExpressionTreeNodeEquals(expr_023, "E or 4", "3");
            var (expr_023E, expr_0234) = VerifyPlaceholders(expr_023, "E", "4");

            builder.Add(_4);
            AssertExpressionTreeNodeEquals(expr_0234, "V or D", "4");
            var (expr_02DV, expr_02DD) = VerifyPlaceholders(expr_0234, "V", "D");
        }

        [Test]
        public void AddExpression_PosTest()
        {
            Assert.That(builder.Roots.Count(), Is.EqualTo(0));
            for(int i = 0; i < expressions.Count; i++)
            {
                builder.Add(expressions[i]);
                Assert.That(builder.Roots.Count(), Is.EqualTo(i+1));
            }
        }
    }
}
