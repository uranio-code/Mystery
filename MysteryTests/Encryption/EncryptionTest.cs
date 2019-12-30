using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Encryption;

namespace MysteryTests.Encryption
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void shouldEncryptDifferently()
        {
            var input = Guid.NewGuid().ToString();
            Assert.AreNotEqual(input.encrypt(), input.encrypt());
            Assert.AreEqual(input.encrypt().decrypt(), input.encrypt().decrypt());
        }
    }
}
