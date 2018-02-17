using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Json;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Json.Tests
{
    [TestClass()]
    public class MongoContentCollectionTests
    {
        [TestMethod()]
        public void MongoContentCollectionContainsTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            var probe = TestContentType.getARandomTestContentType(enforce_a_reference:false);
            Assert.IsFalse(collection.Contains(probe));
            collection.Add(probe);
            Assert.IsTrue(collection.Contains(probe));
        }

        [TestMethod()]
        public void MongoContentCollectionAddTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            var probe = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            collection.Add(probe);
            Assert.IsFalse(collection.isEmpty);
        }

        [TestMethod()]
        public void MongoContentCollectionAddContentsTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            collection.AddContents(from x in Enumerable.Range(0,10) select
                                   TestContentType.getARandomTestContentType(enforce_a_reference: false));
            
            Assert.IsTrue(collection.Count == 10);
        }

        [TestMethod()]
        public void MongoContentCollectionRemoveContentsTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            var probe = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            var contents = new List<IContent>(from x in Enumerable.Range(0, 10)
                                              select TestContentType.getARandomTestContentType(enforce_a_reference: false));
            contents.Add(probe);
            collection.AddContents(contents);
            
            Assert.IsTrue(collection.Contains(probe));
            collection.Remove(probe);
            Assert.IsFalse(collection.Contains(probe));
        }

        [TestMethod()]
        public void MongoContentCollectionClearTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            collection.AddContents(from x in Enumerable.Range(0, 10)
                                   select
  TestContentType.getARandomTestContentType(enforce_a_reference: false));

            Assert.IsFalse(collection.isEmpty);
            collection.Clear();
            Assert.IsTrue(collection.isEmpty);
        }

       
        [TestMethod()]
        public void MongoContentCollectionGetAllTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            var contents = new List<TestContentType>(
                from x in Enumerable.Range(0, 10)
                select TestContentType.getARandomTestContentType(enforce_a_reference: false));
            collection.AddContents(contents);
            var basket_one = new MultiContentReference<TestContentType>(contents);
            var basket_two = new MultiContentReference<TestContentType>(collection.GetAll().Cast<TestContentType>());
            Assert.AreEqual(basket_one,basket_two);
        }

        [TestMethod()]
        public void MongoContentCollectionGetAllByFilterTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            var contents = new List<TestContentType>(
                from x in Enumerable.Range(0, 10)
                select TestContentType.getARandomTestContentType(enforce_a_reference: false));
            for (var i = 0; i < 5; i++) {
                contents[i].a_string = "Pippo";
            }
            collection.AddContents(contents);
            Expression<Func< TestContentType, bool>> expression = x => x.a_string == "Pippo"; 
            Assert.IsTrue(collection.GetAllByFilter(expression).Count() == 5);
        }

        [TestMethod()]
        public void MongoContentCollectionGetContentTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            var probe = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            collection.Add(probe);
            var back_from_collection = collection.GetContent(probe.guid);
            Assert.IsTrue(back_from_collection.samePropertiesValue(probe));
        }

        [TestMethod()]
        public void MongoContentCollectionGetLightContentRerefereceTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            collection.Clear();
            var contents = new List<IContent>(
                from x in Enumerable.Range(0, 10)
                select TestContentType.getARandomTestContentType(enforce_a_reference: false));
            collection.AddContents(contents);
            
            var refs = new List<LightContentReferece>( collection.GetLightContentRereferece());
            var refs_guid = new HashSet<Guid>(from x in refs select x.guid);
            var ref_texts = new HashSet<string>(from x in refs select x.ReferenceText);

            foreach (var c in contents) {
                Assert.IsTrue(refs_guid.Contains(c.guid));
                Assert.IsTrue(ref_texts.Contains(c.ReferenceText));
            }

        }

        [TestMethod()]
        public void MongoContentCollectionRemoveTest()
        {
            var collection = new MongoContentCollection<TestContentType>();
            var probe = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            collection.Clear();
            collection.Add(probe);
            collection.Remove(probe);
            Assert.IsTrue(collection.isEmpty);
        }
    }
}