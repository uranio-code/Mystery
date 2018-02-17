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
    public class DefaultNotifierTests
    {
        [TestMethod()]
        public void DefaultNotifierhallReitriveANotifier()
        {
            (new DefaultNotifier()).sendMessage(null);
            //no crash is good
            Assert.IsTrue(true);
        }

       

        [TestMethod()]
        public void DefaultNotifiersendBaseMessageTest()
        {
            var notifier = new DefaultNotifier();
            notifier.sendMessage(new BaseMessage());
            //no crash is good
            Assert.IsTrue(true);
        }
    }
}