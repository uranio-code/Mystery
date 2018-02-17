using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Files;
using Mystery.UI;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UI.Tests
{
    [TestClass()]
    public class JsSortValueTests
    {
        [TestMethod()]
        public void JsSortValuegetSortValueNullTest()
        {
            //just don't crash
            JsSortValue.getSortValue(null);
            Assert.IsTrue(true);
        }
        [TestMethod()]
        public void JsSortValuegetSortValueBasicTest()
        {
            Assert.AreEqual("hello", JsSortValue.getSortValue("hello"));
            Assert.AreEqual(1.3, JsSortValue.getSortValue(1.3));
            Assert.AreEqual(true, JsSortValue.getSortValue(true));
            Assert.AreEqual(false, JsSortValue.getSortValue(false));
            long the_long = 12345;
            Assert.AreEqual(the_long, JsSortValue.getSortValue(the_long));
        }
        [TestMethod()]
        public void JsSortValuegetSortValueDateTest()
        {
            long today = (long)JsSortValue.getSortValue(DateTime.Now);
            long tomorrow = (long)JsSortValue.getSortValue(DateTime.Now.AddDays(1));
            Assert.IsTrue(today < tomorrow);
        }

        private enum TestEnum {a,b,c}

        [TestMethod()]
        public void JsSortValuegetSortValueTest()
        {
            int a = (int)JsSortValue.getSortValue(TestEnum.a);
            int c = (int)JsSortValue.getSortValue(TestEnum.c);
            Assert.IsTrue(a<c);
        }
        [TestMethod()]
        public void JsSortValuegetSortValueContentTest()
        {
            var c1 = (string)JsSortValue.getSortValue( new TestContentType { the_reference_text = "a" });
            var c2 = (string)JsSortValue.getSortValue(new TestContentType { the_reference_text = "b" });
            Assert.AreEqual(string.Compare(c1,c2),-1);
        }
        [TestMethod()]
        public void JsSortValuegetSortValueContentReferenceTest()
        {
            var c1 = (string)JsSortValue.getSortValue(new ContentReference<TestContentType>(new TestContentType { the_reference_text = "a" }));
            var c2 = (string)JsSortValue.getSortValue(new ContentReference<TestContentType>(new TestContentType { the_reference_text = "b" }));
            Assert.AreEqual(string.Compare(c1, c2), -1);
            
        }
        [TestMethod()]
        public void JsSortValuegetSortValueMysteryFileTest()
        {
            var c1 = (string)JsSortValue.getSortValue(new MysteryFile { filename = "a" });
            var c2 = (string)JsSortValue.getSortValue(new MysteryFile { filename = "b" });
            Assert.AreEqual(string.Compare(c1, c2), -1);
        }
        [TestMethod()]
        public void JsSortValuegetSortValueEmptyListTest()
        {
            var l1 = new List<IContent>();
            Assert.IsTrue(string.IsNullOrEmpty((string)JsSortValue.getSortValue(l1)));
        }
        [TestMethod()]
        public void JsSortValuegetSortValue1elementTest()
        {
            var c1 = (string)JsSortValue.getSortValue(
                new List<IContent> { new TestContentType { the_reference_text = "a" } }
                );
            var c2 = (string)JsSortValue.getSortValue(
                new List<IContent> { new TestContentType { the_reference_text = "b" } });
            Assert.AreEqual(string.Compare(c1, c2), -1);
        }
        [TestMethod()]
        public void JsSortValuegetSortValueMoreElementTest()
        {
            var c1 = (string)JsSortValue.getSortValue(
               new List<IContent> { new TestContentType { the_reference_text = "a" } }
               );
            var c2 = (string)JsSortValue.getSortValue(
                new List<IContent> {
                    new TestContentType { the_reference_text = "a" },
                    new TestContentType { the_reference_text = "b" } });
            Assert.AreEqual(string.Compare(c1, c2), -1);
        }
        [TestMethod()]
        public void JsSortValuegetSortValueSameElementsTest()
        {
            var c1 = (string)JsSortValue.getSortValue(
               new List<IContent> {
                   new TestContentType { the_reference_text = "b" },
                   new TestContentType { the_reference_text = "a" },
               }
               );
            var c2 = (string)JsSortValue.getSortValue(
                new List<IContent> {
                    new TestContentType { the_reference_text = "a" },
                    new TestContentType { the_reference_text = "b" } });
            Assert.AreEqual(string.Compare(c1, c2), 0);
        }

        private class SomethingUnexpected { }

        [TestMethod()]
        public void JsSortValuegetSortValueWhatEverTest()
        {
            //just don't crash
            JsSortValue.getSortValue(new SomethingUnexpected());
            Assert.IsTrue(true);
        }

    }
}