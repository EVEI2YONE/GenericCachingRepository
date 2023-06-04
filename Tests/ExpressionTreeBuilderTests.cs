using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Models.FilterExpressionNamespace;
using Models.FilterExpressionTreeBuildersNamespace;
using Models.FilterExpressionTreesNamespace;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Tests
{
    internal class FilterExpressionTreeBuilderTests
    {
        FilterExpressionTreeBuilder builder;
        List<FilterExpression?> expressions;
        string? message;

        [SetUp]
        public void SetUp()
        {
            message = null;
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
            AssertEquals(expressions.Count(), 8);
        }

        private int Count { get { return builder.Roots.Count(); } }
        private void AssertEquals(object val1, object val2) => Assert.That(val1, Is.EqualTo(val2).IgnoreCase);
        private void AssertCount(int count) => AssertEquals(builder.Roots.Count(), count);
        private void AssertCount(FilterExpressionTree tree, int count) => AssertEquals(FilterExpressionTree.TotalChildren(tree), count);
        private void AssertFilterExpressionTreeNodeEquals(FilterExpressionTree FilterExpressionTree, string value, string name)
        {
            Assert.IsNotNull(FilterExpressionTree);
            Assert.IsNotNull(FilterExpressionTree.Expression);
            var expression = FilterExpressionTree.Expression;
            AssertEquals(expression.Value, value);
            AssertEquals(expression.Name, name);
        }
        private (FilterExpressionTree? tree1, FilterExpressionTree? tree2) VerifyPlaceholders(FilterExpressionTree? parent, string leftName, string rightName, string? leftValue = null, string? rightValue = null, bool isRoot = false)
        {
            var left = parent.Left;
            var right = parent.Right;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            AssertFilterExpressionTreeNodeEquals(left, leftValue, leftName);
            AssertFilterExpressionTreeNodeEquals(right, rightValue, rightName);
            AssertEquals(parent, left.Parent);
            AssertEquals(parent, right.Parent);
            return (left, right);
        }
        private void ExpectAssertException(FilterExpressionTree left, FilterExpressionTree right)
        {
            Assert.Throws<AssertionException>(() => VerifyPlaceholders(left, null, null));
            Assert.Throws<AssertionException>(() => VerifyPlaceholders(right, null, null));
        }
        private void ExpectException<T>(Action action, string message, int? count = null) where T : Exception
        {
            var exception = Assert.Throws<ArgumentException>(() => action.Invoke());
            AssertEquals(exception.Message, message);
            if (count != null)
                AssertEquals(Count, count.Value);
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
            AssertEquals(node.Expression.Name, "2");
            AssertEquals(node.Expression.Value, "3 or D");
            AssertEquals(node.Parent.Expression.Name, "0");
            AssertEquals(node.Parent.Expression.Value, "1 or 2");
        }

        [Test]
        public void CreateDisjointTrees_PosTest()
        {
            AssertEquals(builder.Roots.Count(), 0);
            BuildFilterExpressionTree_PosTest();
            var _5 = expressions[5];
            var _6 = expressions[6];

            AssertEquals(builder.Roots.Count(), 1);
            builder.Add(_6);
            AssertEquals(builder.Roots.Count(), 2);
        }

        [Test]
        public void ConnectDisjointTrees_PosTest()
        {
            AssertEquals(builder.Roots.Count(), 0);
            BuildFilterExpressionTree_PosTest();
            var _5 = expressions[5];
            var _6 = expressions[6];
            var _A = expressions[7];

            AssertEquals(builder.Roots.Count(), 1);
            builder.Add(_5);
            AssertEquals(builder.Roots.Count(), 2);
            builder.Add(_6);
            AssertEquals(builder.Roots.Count(), 2);
            builder.Add(_A);
            AssertEquals(builder.Roots.Count(), 1);
        }

        [Test]
        public void NotOperator_PosTest()
        {
            var currentCount = expressions.Count();
            expressions.Add("V: !A or B");
            AssertEquals(expressions.Count(), currentCount + 1);

            expressions.Add("V: !!C or D");
            AssertEquals(expressions.Count(), currentCount + 2);
            Assert.IsNull(expressions.Last());
        }

        [Test]
        public void ConnectDisjointTrees_UnderlyingNullExpression_PosTest()
        {
            AssertEquals(builder.Roots.Count(), 0);
            BuildFilterExpressionTree_PosTest();
            var _5 = expressions[5];
            var _6 = expressions[6];
            expressions.Remove(expressions.Last());
            expressions.Add("A: !6 or G");
            var _A = expressions[7];

            AssertEquals(builder.Roots.Count(), 1);
            builder.Add(_5);
            AssertEquals(builder.Roots.Count(), 2);
            builder.Add(_6);
            AssertEquals(builder.Roots.Count(), 2);
            builder.Add(_A);
            AssertEquals(builder.Roots.Count(), 1);
        }

        [Test]
        public void ConnectDisjointTrees_DuplicateExpressionName_NegTest()
        {
            //A: 6 or G  => removed
            //A: !6 or G => inserted
            ConnectDisjointTrees_UnderlyingNullExpression_PosTest();

            //A: 6 or G  => fail to inserted
            message = "Expression Name 'A' matches 'A'";
            ExpectException<ArgumentException>(() => builder.Add("A: 6 or G"), message);

            //E: 6 or G  => inserted
            var currentCount = Count;
            builder.Add("E: 6 or G");
            AssertEquals(builder.Roots.Count(), currentCount);
        }

        [Test]
        public void AliasRegistered_PosTest()
        {
            BuildFilterExpressionTree_PosTest();
            //0: 1 or 2
            //Z: 1 or 2

            var _1 = builder.Aliases.FirstOrDefault(x => x.Key == "0").Value;
            var _Z = builder.Aliases.FirstOrDefault(x => x.Key == "Z").Value;
            Assert.IsNotNull(_1);
            Assert.IsNull(_Z);

            var currentCount = Count;
            message = "";
            var exception = Assert.Throws<ArgumentException>(() => builder.Add("Z: 1 or 2"), message, currentCount);
            //Assert.That();
        }

        [Test]
        public void ConnectDisjointTrees_DuplicateExpression_RegisterAlias_NegTest()
        {
            //A: !6 or G => inserted
            //E: 6 or G  => inserted
            ConnectDisjointTrees_DuplicateExpressionName_NegTest();

            //Z: !6 or G => Z matches A
            var currentCount = Count;
            var aliasCount = builder.Aliases.Count();
            message = "Expression 'Z' matches 'A' : '!6 or G'";
            ExpectException<ArgumentException>(() => builder.Add("Z: !6 or G"), message, currentCount);

            AssertEquals(builder.Aliases.Count(), aliasCount+1);
            //Z => A
            var alias = builder.Aliases.FirstOrDefault(x => x.Key == "Z").Value;
            var orig = builder.GetAlias(alias);
            AssertEquals(orig, alias);
            AssertEquals(orig, "A");
        }

        [Test]
        public void ConnectDisjointTrees_Implicit_DuplicateExpression_NegTest()
        {
            //A now has Z as alias
            ConnectDisjointTrees_DuplicateExpression_RegisterAlias_NegTest();
            var alias = builder.Aliases.First(x => x.Key == "Z").Value;
            var orig = builder.GetAlias(alias);
            AssertEquals(orig, "A");

            //A = Z
            //1: A or B => currently inserted
            //V: Z or B => 'V' matches '1' because 'Z = A' => 'A or B'
            message = "Expression 'V' matches '1' : 'A or B'";
            var count = Count;
            ExpectException<ArgumentException>(() => builder.Add("V: Z or B"), message, count);
        }
    }
}
