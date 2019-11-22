using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Json;
using Mystery.Users;
using MysteryTests;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Json.Tests
{
    [TestClass()]
    public class MysteryJsonConverterTests : BaseMysteryTest
    {

        [TestMethod()]
        public void ShouldBeAbleToSerializeAContent()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            string result = converter.getJson(TestContentType.getARandomTestContentType(enforce_a_reference: false));
            Assert.IsTrue(result.Length > 0);
        }
        [TestMethod()]
        public void ShouldBeAbleToAvoidRecursion()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            TestContentType adriano = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            adriano.single_reference = adriano;
            string result = converter.getJson(adriano);
            Assert.IsTrue(result.Length > 0);
        }
        [TestMethod()]
        public void ShouldBeAbleToDeserializeToANotContent()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            TestContentType adri = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            string json = converter.getJson(adri);
            TestNotContentType nc_adri = Newtonsoft.Json.JsonConvert.DeserializeObject<TestNotContentType>(json);
            Assert.AreEqual(nc_adri.a_boolean, adri.a_boolean);
            Assert.AreEqual(nc_adri.a_double, adri.a_double);
            Assert.AreEqual(nc_adri.a_integer, adri.a_integer);
            Assert.AreEqual(nc_adri.a_string, adri.a_string);
            Assert.AreEqual(adri.single_reference,nc_adri.single_reference);
            Assert.IsTrue((from x in nc_adri.multi_reference select x.guid).sameContents(from x in adri.multi_reference select x.guid.Value));
        }

        [TestMethod()]
        public void readContentTestWithoutReference()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            TestContentType adriano = TestContentType.getARandomTestContentTypeWithoutreference();
            string json = converter.getJson(adriano);
            TestContentType result = converter.readJson<TestContentType>(json);
            Assert.IsTrue(result.samePropertiesValue(adriano));
        }
        [TestMethod()]
        public void readContentTestWithOrWithoutReference()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            TestContentType adriano = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            string json = converter.getJson(adriano);
            TestContentType result = converter.readJson<TestContentType>(json);
            Assert.IsTrue(result.samePropertiesValue(adriano));
        }
        [TestMethod()]
        public void readContentTestWithReference()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            TestContentType adriano = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            string json = converter.getJson(adriano);
            TestContentType result = converter.readJson<TestContentType>(json);
            Assert.IsTrue(result.samePropertiesValue(adriano));
        }
        [TestMethod()]
        public void shouldBeAbleToKnowTheTypeFromJson()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            TestContentType adriano = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            string json = converter.getJson(adriano);
            IContent result = converter.readJson<IContent>(json);
            Assert.IsTrue(result is TestContentType);
        }

        [TestMethod()]
        public void getJsonShouldBeHandleNullTest()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            Assert.IsNotNull(converter.getJson(null));
            TestContentType adriano = null;
            Assert.IsNotNull(converter.getJson(adriano));
        }
        [TestMethod()]
        public void getJsonShouldBeHandleNullInputTest()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            Assert.IsNull(converter.readJson<object>(null));
            Assert.IsNull(converter.readJson<object>(string.Empty));

        }

        [TestMethod()]
        public void MysteryJsonConvertergetEmptyObjectJsonTest()
        {
            IMysteryJsonConverter converter = new MysteryJsonConverter();
            string json = converter.getJson(new object());
            Assert.AreEqual(json,"{}");
        }
    }
}