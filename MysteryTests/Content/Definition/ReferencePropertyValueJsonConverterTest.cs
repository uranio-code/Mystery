using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Register;
using Mystery.Json;

namespace MysteryTests.Content.Definition
{
    [TestClass]
    public class ReferencePropertyValueJsonConverterTest
    {

        public class testClass
        {
            public ContentReference<TestContentType> reference { get; set; }
        }

        [TestMethod]
        public void canCovertContentReference()
        {
            var json = "{}";
            var converter = this.getGlobalObject<MysteryJsonConverter>();
            Assert.IsNotNull(converter.readJson<testClass>(json));
            Assert.IsNull(converter.readJson<testClass>(json).reference);
        }
        [TestMethod]
        public void ContentReferenceAreNotNullOutOfDb()
        {
            var converter = this.getGlobalObject<MysteryJsonConverter>();
            //when we deserialize for convenience we instance it
            var json = "{reference:null}";
            Assert.IsNotNull(converter.readJson<testClass>(json).reference);
        }
        [TestMethod]
        public void ContentReferenceCanBeEmptyObjects()
        {
            var converter = this.getGlobalObject<MysteryJsonConverter>();
            //when we deserialize for convenience we instance it
            var json = "{reference:{}}";
            Assert.IsNotNull(converter.readJson<testClass>(json).reference);
            Assert.IsTrue(converter.readJson<testClass>(json).reference.isNullOrEmpty());
        }
        [TestMethod]
        public void ContentReferenceCanContainEmptyGuid()
        {
            var converter = this.getGlobalObject<MysteryJsonConverter>();
            //when we deserialize for convenience we instance it
            var json = "{reference:{guid:null}}";
            Assert.IsNotNull(converter.readJson<testClass>(json).reference);
            Assert.IsTrue(converter.readJson<testClass>(json).reference.isNullOrEmpty());
        }
        [TestMethod]
        public void ContentReferenceCanContainEmptyStringGuid()
        {
            var converter = this.getGlobalObject<MysteryJsonConverter>();
            //when we deserialize for convenience we instance it
            var json = "{reference:{guid:''}}";
            Assert.IsNotNull(converter.readJson<testClass>(json).reference);
            Assert.IsTrue(converter.readJson<testClass>(json).reference.isNullOrEmpty());
        }
    }
}
