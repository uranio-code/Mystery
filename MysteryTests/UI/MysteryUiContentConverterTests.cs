using Microsoft.VisualStudio.TestTools.UnitTesting;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.UiJson;

namespace Mystery.UI.Tests
{
    [TestClass()]
    public class MysteryUiContentConverterTests
    {
        [TestMethod()]
        public void MysteryUiContentConvertergetContentUiTest()
        {
            TestContentType c = TestContentType.getARandomTestContentType(enforce_a_reference:true);
            var cov = new  MysteryUiContentConverter();
            var result = cov.getContentUi(c,new Users.User());
            Assert.IsNotNull(result);
            foreach (var pui in result.propertiesUi.Values) {
                Assert.IsNotNull(pui.template_url);
                Assert.AreNotEqual(pui.template_url,string.Empty);
            }
        }
    }
}