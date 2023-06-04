using Models.FilterExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepositoryTests
{
    internal class ExpressionTests
    {
        private FilterExpression expression;

        [SetUp]
        public void SetUp()
        {
            expression = new FilterExpression();
        }

        [Test]
        public void GetExpressionChildrenNames_PosTest()
        {
            expression.Name = "0";
            expression.Value = "A or B";
            var (left, right) = expression.GetExpressionChildrenNames();
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.That(left, Is.EqualTo("A"));
            Assert.That(right, Is.EqualTo("B"));
        }

        [Test]
        public void GetExpressionChildrenNames_InvalidFormat_NegTest()
        {
            //less than 2
            expression.Name = "0";
            expression.Value = "A or ";
            var ex = Assert.Throws<ArgumentException>(() => expression.GetExpressionChildrenNames());
            var errorMessage = "Expression Value is not in the correct format: 'A and B', 'C or D' (Parameter 'Value')";
            Assert.That(ex.Message, Is.EqualTo(errorMessage));

            //more than 2
            expression.Value = "A or A or B";
            ex = Assert.Throws<ArgumentException>(() => expression.GetExpressionChildrenNames());
            Assert.That(ex.Message, Is.EqualTo(errorMessage));

            //more than 2 expressions
            expression.Value = "A or A B";
            ex = Assert.Throws<ArgumentException>(() => expression.GetExpressionChildrenNames());
            Assert.That(ex.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public void GetExpressionChildrenNames_DuplicateRule_NegTest()
        {
            expression.Name = "0";
            expression.Value = "A or A";
            var ex = Assert.Throws<ArgumentException>(() => expression.GetExpressionChildrenNames());
            var errorMessage = $"Expression is duplicated '0 : A or A' (Parameter 'FilterExpression')";
            Assert.That(ex.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public void GetExpressionRule()
        {
            //expression.
        }

        [Test]
        public void ResolveExpressions()
        {
            throw new NotImplementedException();
        }
    }
}
