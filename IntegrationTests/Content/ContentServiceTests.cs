using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.UI;
using Mystery.UiJson;
using Mystery.Web;
using MysteryTests.Content.Definition;
using MysteryWebLogic.Authetication;
using MysteryWebLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Content
{
    [TestClass()]
    public class ContentServiceTests
    {
        [TestMethod()]
        public void ContentServiceHelloTest()
        {
            MysterySession session = this.getGlobalObject<MysterySession>();
            session.authenticated_user = new Mystery.Users.User();
            var s = new ContentService();
            Assert.IsFalse(string.IsNullOrEmpty(s.Hello()));
        }

        [TestMethod()]
        public void ContentServiceContentTest()
        {
            var c = TestContentType.getARandomTestContentType(enforce_a_reference: false);
            //let's avoid single reference
            c.single_reference = new ContentReference<TestContentType>();
            ContentsDatabase db = this.getGlobalObject<ContentsDatabase>();
            db.Add(c);
            db.AddContents(c.multi_reference.getAsContentEnum());
            var uid = c.guid.Tiny();
            var s = new ContentService();
            var service_result = s.ContentView(nameof(TestContentType),uid);
            var converter = this.getGlobalObject<IMysteryJsonUiConverter>();
            Assert.IsTrue(converter.readJson<IContent>(service_result.json_output) is TestContentType);
        }

        [TestMethod()]
        public void ContentServiceContentViewTest()
        {
            var c = TestContentType.getARandomTestContentType(enforce_a_reference: true);
            var cd = this.getGlobalObject<IContentDispatcher>();
            cd.Add(c);
            cd.AddContents(c.single_reference.getAsContentEnum());
            cd.AddContents(c.multi_reference.getAsContentEnum());
            var uid = c.guid.Tiny();
            var s = new ContentService();
            var service_result = s.ContentView(nameof(TestContentType),uid);
            var converter = this.getGlobalObject<IMysteryJsonUiConverter>();
            var cui = converter.readJson<ContentUi>(service_result.json_output);
            foreach (PropertyView property_view in typeof(TestContentType).getMysteryPropertyAttributes<PropertyView>())
            {
                var cp = property_view.used_in.getMysteryAttribute<ContentProperty>();
                string name = cp == null ? property_view.used_in.Name : cp.name;

                Assert.IsTrue(cui.propertiesUi.ContainsKey(name));
                Assert.AreEqual(cui.propertiesUi[name].content.property_name, name);
            }

            Assert.AreEqual(cui.propertiesUi[nameof(TestContentType.a_string)].content.property_value, c.a_string);
            Assert.AreEqual(cui.propertiesUi[nameof(TestContentType.a_integer)].content.property_value, c.a_integer);
            Assert.AreEqual(cui.propertiesUi[nameof(TestContentType.a_double)].content.property_value, c.a_double);
            Assert.AreEqual(cui.propertiesUi[nameof(TestContentType.a_boolean)].content.property_value, c.a_boolean);
            Assert.AreEqual(cui.propertiesUi[nameof(TestContentType.a_enum)].content.property_value.ToString(), c.a_enum.ToString());

        }
    }
}