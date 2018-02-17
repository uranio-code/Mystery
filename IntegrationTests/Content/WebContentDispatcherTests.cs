using Microsoft.VisualStudio.TestTools.UnitTesting;
using MysteryTests.Content.Containers;
using MysteryWebLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Content;
using MysteryTests.Content.Definition;
using Mystery.Register;
using MysteryWebLogic.Authetication;
using Mystery.MysteryAction;
using Mystery.Web;
using Mystery.Json;

namespace IntegrationTests.Content
{
    [TestClass()]
    public class WebContentDispatcherTests 
    {


        [ClassCleanup]
        static public void WebContentDispatcherTestsCleanUp()
        {
            
        }
        #region BaseContainerTests

        private class TestMakingAction : BaseMysteryAction<String, String>
        {
            private Func<string> _test;
            public TestMakingAction(Func<string> test):base() {
                _test = test;
            }

            protected override ActionResult<string> ActionImplemetation()
            {
                string result = _test();
                return new ActionResult<string>()
                {
                    isSuccessfull = true,
                    message = result,
                    output = "cool",
                    UnAuthorized = false
                };
            }

            protected override bool AuthorizeImplementation()
            {
                return true;
            }
        }

        static Random rnd = new Random();


        private string ExecuteTest(Func<string> test) {
            MysterySession session = this.getGlobalObject<MysterySession>();
            session.authenticated_user = new Mystery.Users.User();

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new TestMakingAction(test), "").message;
            }
        }
        private void ExecuteTest(Action test)
        {
            Func<string> to_test = () =>
            {
                test();
                return "cool";
            };
            MysterySession session = this.getGlobalObject<MysterySession>();
            session.authenticated_user = new Mystery.Users.User();

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                executor.executeAction(new TestMakingAction(to_test), "");
            }
        }



        [TestMethod()]
        public void WebContentDispatcherTestEnvTest()
        {
            //yeah a test which test the tests code
            bool executed = false;
            ExecuteTest(() => { executed = true; });
            Assert.IsTrue(executed);
        }

        [TestMethod()]
        public void WebContentDispatcherNoExceptionTest()
        {
            IContentDispatcher container = null;
            WebActionExecutor executor=null;
            ContentsDatabase db = this.getGlobalObject< ContentsDatabase>();
            TestContentType c = null;
            ExecuteTest(() => {
                container = WebContentDispatcher.getDispatcher();
                c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                container.Add(c);
                Guid guid = c.guid;
                executor = WebActionExecutor.current;
                return guid.Tiny();
            });
            Assert.IsTrue(executor.status == WebActionExecutorStatus.done);
            Assert.IsTrue(db.Contains(c));
        }

        [TestMethod()]
        public void WebContentDispatcherExceptionTest()
        {
            IContentDispatcher container = null;
            WebActionExecutor executor = null;
            ContentsDatabase db = this.getGlobalObject<ContentsDatabase>();
            TestContentType c = null;
            ExecuteTest(() => {
                container = WebContentDispatcher.getDispatcher();
                c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                container.Add(c);
                Guid guid = c.guid;
                executor = WebActionExecutor.current;
                throw new Exception("stop now!");
            });
            Assert.IsTrue(executor.status == WebActionExecutorStatus.error);
            Assert.IsFalse(db.Contains(c));
        }

        [TestMethod()]
        public void WebContentDispatcherAddTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
                TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                container.Add(c);
                Guid guid = c.guid;
                Assert.AreSame(c, container.GetContent<TestContentType>(guid));
                container.Dispose();
                return guid.Tiny();
            });
        }

        [TestMethod()]
        public void WebContentDispatcheraddContentsTest()
        {
            ExecuteTest(() => {

                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
                List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                             select TestContentType.getARandomTestContentType(enforce_a_reference: false));

                container.AddContents(contents);

                Assert.AreEqual(container.Count, contents.Count);
                container.Dispose();
            });
        }

        [TestMethod()]
        public void WebContentDispatcherClearTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
                List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                             select TestContentType.getARandomTestContentType(enforce_a_reference: false));

                container.AddContents(contents);

                container.Clear();
                Assert.AreEqual(container.Count, 0);
                container.Dispose();
            });
        }

        [TestMethod()]
        public void WebContentDispatcherContainsTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
            List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                         select TestContentType.getARandomTestContentType(enforce_a_reference: false));

            Assert.IsFalse(container.Contains(contents[rnd.Next(contents.Count - 1)]));

            container.AddContents(contents);

            Assert.IsTrue(container.Contains(contents[rnd.Next(contents.Count - 1)]));
                container.Dispose();
            });
        }

        [TestMethod()]
        public void WebContentDispatchercontainsTypeTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            Assert.IsFalse(container.ContainsType<TestContentType>());
            container.Add(c);
            Assert.IsTrue(container.ContainsType<TestContentType>());
                container.Dispose();
            });
        }

        [TestMethod()]
        public void WebContentDispatchergetAllTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
                this.setGlobalObject<IContentDispatcher>(container);
                List<IContent> contents = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                             select TestContentType.getARandomTestContentType(enforce_a_reference: false));

                container.AddContents(contents);

                List<IContent> type2 = new List<IContent>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                          select new TestContentType2());
                container.AddContents(type2);

                Assert.IsTrue(container.GetAll<TestContentType2>().sameContents(type2));
                Assert.IsTrue(container.GetAll<TestContentType>().sameContents(type2.Union(contents)));

                container.Dispose();
                this.setGlobalObject<IContentDispatcher>(null);
            });
        }

        [TestMethod()]
        public void WebContentDispatchergetAllByFilterTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
            List<TestContentType> contents = new List<TestContentType>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                                       select TestContentType.getARandomTestContentType(enforce_a_reference: false));

            container.AddContents(contents);

            
            Assert.IsTrue(container.GetAllByFilter< TestContentType>(
                x=>x.a_string == contents[0].a_string || x.a_string == contents[1].a_string)
                .sameContents(contents.Take(2)));
                container.Dispose();
            });
        }


        [TestMethod()]
        public void WebContentDispatcherRemoveTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            container.Add(c);
            Assert.IsTrue(container.Remove(c));
            Assert.AreEqual(container.Count, 0);
            c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            Assert.IsFalse(container.Remove(c));
            Assert.AreEqual(container.Count, 0);
                container.Dispose();
            });
        }

        [TestMethod()]
        public void WebContentDispatchersearchTest()
        {
            ExecuteTest(() => {
                WebContentDispatcher container = new WebContentDispatcher(new InMemoryContainer());
            List<TestContentType> contents = new List<TestContentType>(from int i in Enumerable.Range(0, rnd.Next(10) + 2)
                                                                       select TestContentType.getARandomTestContentType(enforce_a_reference: false));

            container.AddContents(contents);
            Assert.IsTrue(container.Search(contents[0].the_reference_text,10).Count() == 1);
            Assert.IsTrue(container.Search(contents[0].the_reference_text.Replace("-", " "),10).Count() >= 1);
            Assert.IsTrue(container.Search(contents[0].the_reference_text.Split('-')[0],10).Count() >= 1);
                container.Dispose();
            });
        }
        #endregion
        [TestMethod()]
        public void WebContentDispatcherRetriveFromPermanetTest()
        {
            ExecuteTest(() => {
                IContentContainer storage = new InMemoryContainer();
            WebContentDispatcher container = new WebContentDispatcher(storage);

            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            storage.Add(c);
            Guid guid = c.guid;
            Assert.IsNotNull(container.GetContent<TestContentType>(c.guid));
                container.Dispose();
            });
            
        }
        [TestMethod()]
        public void WebContentDispatcherWhenRemovedStorageStilHaveIt()
        {
            ExecuteTest(() => {
                IContentContainer storage = new InMemoryContainer();
                WebContentDispatcher container = new WebContentDispatcher(storage);
                this.setGlobalObject<IContentDispatcher>(container);

                TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
                storage.Add(c);
           
                Assert.IsTrue(container.Remove(c));

                Assert.IsFalse(container.Contains(c));
                Assert.IsTrue(storage.Contains(c));
                container.Dispose();

                this.setGlobalObject<IContentDispatcher>(null);
            });

        }
        [TestMethod()]
        public void WebContentDispatcherStorageHaveDifferentInstaceButSameProperties()
        {
            ExecuteTest(() => {
                IContentContainer storage = new InMemoryContainer();
            WebContentDispatcher container = new WebContentDispatcher(storage);

            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            storage.Add(c);
            Guid guid = c.guid;

            Assert.AreNotSame(container.GetContent<TestContentType>(guid),
                storage.GetContent<TestContentType>(guid));


            Assert.IsTrue(container.GetContent<TestContentType>(guid).samePropertiesValue(storage.GetContent<TestContentType>(guid)));
                container.Dispose();
            });
        }

        [TestMethod()]
        public void WebContentDispatcherStorageInstanceUpdateOnDispose()
        {
            
            ExecuteTest(() => {
                IContentContainer storage = new InMemoryContainer();
                string test_value = "ciao";
                WebContentDispatcher container = new WebContentDispatcher(storage);
                this.getMystery().AssemblyRegister.setGlobalObject<IContentDispatcher>(container);

                TestContentType c = TestContentType.getARandomTestContentTypeWithoutreference();
                storage.Add(c);
                Guid guid = c.guid;
                TestContentType in_request = container.GetContent<TestContentType>(guid);
                in_request.a_string = test_value;
                Assert.AreNotEqual(test_value,c.a_string);
                container.Dispose();
                c = storage.GetContent<TestContentType>(guid);
                Assert.AreEqual(test_value, c.a_string);
                this.getMystery().AssemblyRegister.setGlobalObject<IContentDispatcher>(null);
            });
        }

        [TestMethod()]
        public void WebContentDispatcherStorageInstanceDeletedOnDispose()
        {

            ExecuteTest(() => {
                IContentContainer storage = new InMemoryContainer();
                
                WebContentDispatcher container = new WebContentDispatcher(storage);
                this.getMystery().AssemblyRegister.setGlobalObject<IContentDispatcher>(container);

                TestContentType c = TestContentType.getARandomTestContentTypeWithoutreference();
                storage.Add(c);
                container.Remove(c);
                
                Assert.IsTrue(storage.Contains(c));
                container.Dispose();
                
                Assert.IsFalse(storage.Contains(c));
                this.getMystery().AssemblyRegister.setGlobalObject<IContentDispatcher>(null);
            });
        }

        [TestMethod()]
        public void WebContentDispatcherStorageInstanceAddedOnDispose()
        {

            ExecuteTest(() => {
                IContentContainer storage = new InMemoryContainer();

                WebContentDispatcher container = new WebContentDispatcher(storage);
                this.getMystery().AssemblyRegister.setGlobalObject<IContentDispatcher>(container);

                TestContentType c = TestContentType.getARandomTestContentTypeWithoutreference();
                container.Add(c);
                

                Assert.IsFalse(storage.Contains(c));
                container.Dispose();

                Assert.IsTrue(storage.Contains(c));
                this.getMystery().AssemblyRegister.setGlobalObject<IContentDispatcher>(null);
            });
        }
    }
    

    
}