using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Content.Tests
{
    [TestClass()]
    public class MultiContentReferenceTests
    {
        [TestMethod()]
        public void MultiContentReferenceEqualsTest()
        {
            var first = new MultiContentReference<TestContentType>();
            var second =  new MultiContentReference<TestContentType>();
            first.Add(null);
            Assert.IsTrue(first.Equals(second));
            second.Add(null);
            Assert.IsTrue(first.Equals(second));
            var c = TestContentType.getARandomTestContentTypeWithoutreference();
            first.Add(c);
            Assert.IsFalse(first.Equals(second));
            second.Add(c);
            Assert.IsTrue(first.Equals(second));
            //only distinct count
            second.Add(c);
            Assert.IsTrue(first.Equals(second));
        }
        [TestMethod()]
        public void MultiContentReferenceNotEqualsTest()
        {
            var first = new MultiContentReference<TestContentType>();
            var second = new MultiContentReference<TestContentType>();
            var c = TestContentType.getARandomTestContentTypeWithoutreference();
            second.Add(c);
            Assert.IsFalse(first.Equals(second));
        }
    }
}