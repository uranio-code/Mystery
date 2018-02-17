using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class MysteryExtensionsTests
    {
        [TestMethod()]
        public void MysteryExtensionsTinyTest()
        {
            var guid = Guid.NewGuid();
            Assert.AreEqual(guid, guid.Tiny().fromTiny());
        }
        [TestMethod()]
        public void MysteryExtensionsTinyTestWronglength()
        {
            Assert.AreEqual(Guid.Empty, "ciao".fromTiny());
        }
        [TestMethod()]
        public void MysteryExtensionsTinyTestWrongContent()
        {
            //wCMa0uDHxk2rlS7ssK3pww would be valid
            var s = "wCMa0uDHxk2rlS7ssK3p{}";
            Assert.AreEqual(Guid.Empty, s.fromTiny());
        }

        [TestMethod()]
        public void MysteryExtensionslogTest()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex) {
                this.log().Error(ex);
            }
            
            Assert.IsTrue(true);
        }
    }
}