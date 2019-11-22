using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Register;
using Mystery.UI;
using MysteryTests.Content.Definition;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Content;

namespace Mystery.UI.Tests
{
    [TestClass()]
    public class MysteryJsonUiConverterTests
    {
        [TestMethod()]
        public void MysteryJsonUiConvertergetJsonTest()
        {
            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            var cov = new MysteryJsonUiConverter();
            var json = cov.getJson(c);

            var jo = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);
            JObject propertiesUi = (JObject)jo.GetValue(nameof(ContentUi.propertiesUi));

            //to go down to the value
            JObject ref_ui = (JObject)propertiesUi.GetValue(nameof(TestContentType.single_reference));
            ref_ui = (JObject)ref_ui.GetValue("content");
            ref_ui = (JObject)ref_ui.GetValue(nameof(TestContentType.single_reference));

            string ct_name = (string)ref_ui.GetValue(nameof(ContentType));

            Assert.AreEqual(ct_name, nameof(TestContentType));

        }

        
    }
}