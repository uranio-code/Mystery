using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using MysteryTests;
using MysteryTests.Content.Definition;
using System.Collections.Generic;
using System.Linq;
using Mystery.Register;
using MysteryWebLogic.Authetication;
using Mystery.Web;
using Mystery.Json;

namespace IntegrationTests
{
    [TestClass()]
    public abstract class ContainerTests : BaseMysteryTest
    {
        abstract protected IContentContainer getContainer();

        static Random rnd = new Random();

        [TestMethod()]
        public void getContentShouldBeNullIfUnknowTest()
        {
            IContentContainer container = getContainer();
            Assert.IsNull(container.GetContent<BaseContent>(Guid.NewGuid()));
        }

        [TestMethod()]
        public void AddTest()
        {
            IContentContainer container = getContainer();
            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            container.Add(c.single_reference.value);
            foreach (var r in c.multi_reference) container.Add(r.value);
            container.Add(c);
            Guid guid = c.guid;
            var back_from_container = container.GetContent<TestContentType>(guid);
            Assert.AreEqual(c.guid, back_from_container.guid);
            Assert.IsTrue(c.samePropertiesValue(back_from_container));
            Assert.AreEqual(c.GetType(), back_from_container.GetType());
        }

        [TestMethod()]
        public void addContentsTest()
        {

            IContentContainer container = getContainer();
            List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                         select TestContentType.getARandomTestContentType(enforce_a_reference: false));

            container.AddContents(contents);

            Assert.AreEqual(container.Count, contents.Count);

        }

        [TestMethod()]
        public void ClearTest()
        {

            IContentContainer container = getContainer();
            List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                         select TestContentType.getARandomTestContentType(enforce_a_reference: false));

            container.AddContents(contents);

            container.Clear();
            Assert.AreEqual(container.Count, 0);
        }

        [TestMethod()]
        public void ContainsTest()
        {

            IContentContainer container = getContainer();
            List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                         select TestContentType.getARandomTestContentType(enforce_a_reference: false));

            Assert.IsFalse(container.Contains(contents[rnd.Next(contents.Count - 1)]));

            container.AddContents(contents);

            Assert.IsTrue(container.Contains(contents[rnd.Next(contents.Count - 1)]));
        }

        [TestMethod()]
        public void containsTypeTest()
        {
            IContentContainer container = getContainer();
            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            Assert.IsFalse(container.ContainsType<TestContentType>());
            container.Add(c);
            Assert.IsTrue(container.ContainsType<TestContentType>());
        }

       

        [TestMethod()]
        public void getAllTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor()) {
                IContentContainer container = getContainer();
                container.Clear();
                List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                             select TestContentType.getARandomTestContentType(enforce_a_reference: false));

                container.AddContents(contents);

                List<IContent> type2 = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                          select new TestContentType2());
                container.AddContents(type2);

                Assert.IsTrue(container.GetAll<TestContentType2>().sameContents(type2));
                Assert.IsTrue(container.GetAll<TestContentType>().sameContents(type2.Union(contents)));
            }
            
        }

        [TestMethod()]
        public void getAllByFilterTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor()){
                IContentContainer container = getContainer();
                List<TestContentType> contents = new List<TestContentType>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                                           select TestContentType.getARandomTestContentType(enforce_a_reference: false));

                container.AddContents(contents);


                Assert.IsTrue(container.GetAllByFilter<TestContentType>(
                x => x.a_string == contents[0].a_string || x.a_string == contents[1].a_string)
                .sameContents(contents.Take(2)));
            }
            
        }


        [TestMethod()]
        public void GetEnumeratorTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor()) {
                IContentContainer container = getContainer();
                List<TestContentType> contents = new List<TestContentType>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                                           select TestContentType.getARandomTestContentType(enforce_a_reference: false));

                container.AddContents(contents);
                int tot = 0;
                foreach (IContent c in container) tot += 1;

                Assert.AreEqual(contents.Count, tot);
            }
            
        }

        [TestMethod()]
        public void getLightContentRerefereceTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor()) {
                IContentContainer container = getContainer();
                TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                container.Add(c);
                LightContentReferece light_reference = container.GetLightContentRereferece<TestContentType>().FirstOrDefault();

                Assert.AreEqual(light_reference.ReferenceText, c.ReferenceText);
                Assert.AreEqual(light_reference.guid, c.guid);
            }
            
        }

        [TestMethod()]
        public void RemoveTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor()) {
                IContentContainer container = getContainer();
                TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                container.Add(c);
                Assert.IsTrue(container.Remove(c));
                c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                Assert.IsFalse(container.Remove(c));
                Assert.AreEqual(container.Count, 0);
            }
            
        }

        //[TestMethod()]
        //public void searchTest()
        //{
        //    using (WebActionExecutor executor = new WebActionExecutor()) {
        //        IContentContainer container = getContainer();
        //        List<TestContentType> contents = new List<TestContentType>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
        //                                                                   select TestContentType.getARandomTestContentType(enforce_a_reference: false));

        //        container.AddContents(contents);
        //        Assert.IsTrue(container.Search(contents[0].the_reference_text).Count() == 1);
        //        Assert.IsTrue(container.Search(contents[0].the_reference_text.Replace("-", " ")).Count() >= 1);
        //        Assert.IsTrue(container.Search(contents[0].the_reference_text.Split('-')[0]).Count() >= 1);
        //    }
            
        //}

        [TestMethod()]
        public void ShouldBeDisposable()
        {
            using (WebActionExecutor executor = new WebActionExecutor()) { }
            using (IContentContainer c = getContainer()) { }
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    public class ContentMemoryTest : ContainerTests
    {
       

        protected override IContentContainer getContainer()
        {
            var result = new ContentMemory();
            result.Clear();
            return result;
        }
    }
    [TestClass]
    public class ContentsDatabaseTest : ContainerTests
    {


        protected override IContentContainer getContainer()
        {
            var result = new ContentsDatabase() { can_do_crazy = true };
            result.Clear();
            return result;
        }
    }
}
