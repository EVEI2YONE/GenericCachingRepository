using Models.DictionaryNamespace;
using Models.FilterExpressionNamespace;
using Models.FilterExpressionTreesNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class TreeDictionaryTests
    {
        private FilterExpressionTreeDictionary treeDictionary;
        private List<FilterExpressionTree> trees;

        [SetUp]
        public void SetUp()
        {
            treeDictionary = new FilterExpressionTreeDictionary();
            trees = new List<FilterExpressionTree>();
        }

        private int TreeCount => treeDictionary.Count;
        private int RootCount => treeDictionary.RootKeys.Count();
        private void InsertExpressions(params string[] expressions)
            => expressions.ToList().ForEach(
                x => trees.Add(new FilterExpressionTree() { Expression = (FilterExpression) x}));

        private void AssertEquals(object item1, object item2) => Assert.That(item1, Is.EqualTo(item2));

        [Test]
        public void ExtractKeys_PosTest()
        {
            InsertExpressions("0: 1 or 2");

            treeDictionary.Add("0", "0", trees.First());
            AssertEquals(TreeCount, 1);
            AssertEquals(RootCount, 1);
            var key = "0:0";
            var tree = treeDictionary[key];
            Assert.IsNotNull(tree);
            Assert.AreEqual(tree, trees.First());
        }
    }
}
