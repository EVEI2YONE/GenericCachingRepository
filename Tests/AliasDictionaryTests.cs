using Models.DictionaryNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class AliasDictionaryTests
    {
        private AliasDictionary dictionary;

        [SetUp]
        public void SetUp()
        {
            dictionary = new AliasDictionary();
        }

        [Test]
        public void Insert_PosTest()
        {
            Assert.DoesNotThrow(() => dictionary["A"] = "A");
            Assert.DoesNotThrow(() => dictionary["B"] = "A");
            Assert.DoesNotThrow(() => dictionary["b"] = "A");
        }

        [Test]
        public void Insert_AliasExists_NegTest()
        {
            Insert_PosTest();
            var exception = Assert.Throws<ArgumentException>(() => dictionary["A"] = "B");
            var message = "Invalid operation: Alias['A'] = 'B', because 'A' already exists";
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void Insert_NullKey_NegTest()
        {
            Insert_PosTest();
            string? key = null;
            var exception = Assert.Throws<ArgumentNullException>(() => dictionary[key] = "B");
            var message = new ArgumentNullException("name").Message;
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void Insert_NullValue_NegTest()
        {
            Insert_PosTest();
            string? value = null;
            var exception = Assert.Throws<ArgumentNullException>(() => dictionary["Z"] = value);
            var message = new ArgumentNullException("value").Message;
            Assert.That(exception.Message, Is.EqualTo(message));
        }
    }
}
