using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Register;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Content.Tests
{
    [TestClass()]
    public class BaseContentCreatorTests
    {
        [TestMethod()]
        public void BaseContentCreatorcreateTest()
        {
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var content = cc.getNewContent<TestContentType>();

            Assert.IsNotNull(content.single_reference);
            Assert.IsNotNull(content.multi_reference);
        }
    }
}