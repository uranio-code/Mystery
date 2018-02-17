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
    public class WebActionLinkedObjectFactoryTests
    {

        private class disposable : IDisposable
        {
            public void Dispose(){}
        }



        private class TestClass : WebActionLinkedObjectFactory<disposable>
        {
            protected override disposable makeInstance()
            {
                return new disposable();
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(WebActionLinkedObjectException))]
        public void ShallFailIfCalledBeforeUsingWebActionExecutor()
        {
            TestClass probe = new TestClass();
            probe.getInstance();
            
            //no exception? we failed
            Assert.Fail();
        }
        [TestMethod()]
        [ExpectedException(typeof(WebActionLinkedObjectException))]
        public void ShallFailIfCalledAfterUsingWebActionExecutor()
        {
            TestClass probe = new TestClass();
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                
            }
            probe.getInstance();
            //no exception? we failed
            Assert.Fail();
        }

        [TestMethod()]
        public void ShouldReturnAValueInBlock()
        {
            TestClass probe = new TestClass();
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                Assert.IsNotNull(probe.getInstance());
            }
        }

        [TestMethod()]
        public void ShouldChangeValueDifferentBlocks()
        {
            TestClass probe = new TestClass();
            object one;
            object two;
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                one = probe.getInstance();
            }
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                two = probe.getInstance();
            }
            Assert.AreNotSame(one, two);
        }

        public void ShouldKeepValueinBlocks()
        {
            TestClass probe = new TestClass();
            object one;
            object two;
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                one = probe.getInstance();
                two = probe.getInstance();
            }
            
            Assert.AreSame(one, two);
        }

       
        [TestMethod()]
        public void ShouldChangeValueDifferentInDifferentThread()
        {
            TestClass probe = new TestClass();
            object one;
            object two = null;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                one = probe.getInstance();
                Thread t = new Thread(
                    () => {
                        using (WebActionExecutor thread_executor = new WebActionExecutor())
                        {
                            two = probe.getInstance();
                        }
                    }
                    );
                t.Start();
                t.Join();
            }
            Assert.AreNotSame(one,two);
        }
    }
}