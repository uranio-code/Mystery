using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Messaging;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Messaging.Tests
{
    [TestClass()]
    public class MongoMockNotifierTests
    {
        [TestMethod()]
        public void MongoMockNotifierShallReitriveANotifier()
        {
            (new MongoMockNotifier()).sendMessage(null);
            //no crash is good
            Assert.IsTrue(true);
        }


        [TestMethod()]
        public void MongoMockNotifiersendBaseMessageTest()
        {
            var notifier = new MongoMockNotifier();
            notifier.sendMessage(new BaseMessage());
            //no crash is good
            Assert.IsTrue(true);
        }
    }
}