using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Register;
using MysteryTests;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Content.Tests
{
    [TestClass()]
    public class ContentExtensionsTests : BaseMysteryTest
    {

        [TestMethod()]
        public void samePropertyValueTest()
        {
            TestContentType probe = TestContentType.getARandomTestContentType(enforce_a_reference: true);

            Assert.IsTrue(probe.samePropertiesValue(probe));
        }

        [TestMethod()]
        public void ContentExtensionsareEqualTest()
        {
            TestContentType probe = null;

            ContentReference<TestContentType> one = null;

            //both null
            //shall not crash even if the given object is null
            Assert.IsTrue(one.isSameAs(probe));
            Assert.IsTrue(probe.isReferedBy(one));

            //one null but nothing given
            one = new ContentReference<TestContentType>();
            Assert.IsTrue(one.isSameAs(probe));
            Assert.IsTrue(probe.isReferedBy(one));

            //with an empty Guid
            one.guid = Guid.Empty;
            Assert.IsTrue(one.isSameAs(probe));
            Assert.IsTrue(probe.isReferedBy(one));

            //OK let's have a content
            probe = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            one = new ContentReference<TestContentType>(probe);
            Assert.IsTrue(one.isSameAs(probe));
            Assert.IsTrue(probe.isReferedBy(one));

        }

        [TestMethod()]
        public void ContentExtensionsareEqualTest1()
        {
            TestContentType probe = TestContentType.getARandomTestContentType(enforce_a_reference: true);

            ContentReference<TestContentType> one = null;
            ContentReference<TestContentType> two = null;

            //both null
            //shall not crash even if the given object is null
            Assert.IsTrue(one.isSameAs(two));
            Assert.IsTrue(two.isSameAs(one));

            //one null but nothing given
            one = new ContentReference<TestContentType>();
            Assert.IsTrue(one.isSameAs(two));
            Assert.IsTrue(two.isSameAs(one));

            //with an empty Guid
            one.guid = Guid.Empty;
            Assert.IsTrue(one.isSameAs(two));
            Assert.IsTrue(two.isSameAs(one));

            //let's reference a content
            one = new ContentReference<TestContentType>(probe);
            Assert.IsFalse(one.isSameAs(two));
            Assert.IsFalse(two.isSameAs(one));

            //all right!
            two = new ContentReference<TestContentType>(probe);
            Assert.IsTrue(one.isSameAs(two));
            Assert.IsTrue(two.isSameAs(one));

        }

        
    }
}