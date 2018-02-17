using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Web;
using MysteryWebLogic.Authetication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MysteryWebLogic.Authetication.Tests
{
    [TestClass()]
    public class WebActionExecutorTests
    {
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void youCannotBeingUsing2WebActionExecutor()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                using (WebActionExecutor executor2 = new WebActionExecutor())
                {
                }
            }
            //no exception? we failed
            Assert.Fail();
        }

        [TestMethod()]
        public void CurrentWebActionExecutorShouldBeNothing()
        {
            Assert.IsNull(WebActionExecutor.current);
        }

        [TestMethod()]
        public void WebActionExecutorShallAutoSetWhenInstanced()
        {
            using (WebActionExecutor executor = new WebActionExecutor()) {
                Assert.AreSame(WebActionExecutor.current,executor);
            }
        }
        [TestMethod()]
        public void WebActionExecutorShallAutoDeregister()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                
            }
            Assert.IsNull(WebActionExecutor.current);
        }
        [TestMethod()]
        public void DifferentWebActionExecutorShallCoexsitInDifferentThread()
        {
            bool test_pass_in_different_thread = false;
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                
                Assert.AreSame(WebActionExecutor.current, executor);
                Thread t = new Thread(
                    ()=> {
                        using (WebActionExecutor thread_executor = new WebActionExecutor())
                        {
                            test_pass_in_different_thread = 
                            !ReferenceEquals(WebActionExecutor.current, executor)
                            &&
                            ReferenceEquals(WebActionExecutor.current, thread_executor);
                        }
                    }
                    );
                t.Start();
                t.Join();
            }
            Assert.IsTrue(test_pass_in_different_thread);
        }
    }
}