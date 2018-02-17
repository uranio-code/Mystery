using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mystery.Web;
using Mystery.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Register;
using Mystery.Content;
using MysteryTests.Content.Definition;
using Mystery.Json;

namespace IntegrationTests.UI
{
    [TestClass()]
    public class ContentActionTest
    {
        //[TestMethod()]
        //public void TestPropertyEditAction()
        //{
        //    //argh can manage to fake a request buffer

        //    IGlobalContentCreator cc = this.getGlobalObject<IGlobalContentCreator>();
        //    IContent c = TestContentType.getARandomTestContentType(enforce_a_reference:false); 
        //    IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
        //    cd.Add(c);
        //    var converter = this.getGlobalObject<MysteryJsonConverter>();
        //    var new_value = Guid.NewGuid().ToString();
        //    StringBuilder output = new StringBuilder();
        //    using (StringWriter sw = new StringWriter(output))
        //    {
        //        HttpResponse response = new HttpResponse(sw);
        //        HttpRequest request = new HttpRequest(string.Empty, "http://mytest.local", string.Empty);
        //        var input_json = converter.getJson(new MonoPropertyContent()
        //        {
        //            guid = c.guid,
        //            property_name = nameof(TestContentType.a_string),
        //            property_value = new_value,
        //        });
        //        var requestBody = Encoding.UTF8.GetBytes(input_json);
        //        request.InputStream.Write(requestBody, 0, requestBody.Length);
        //        request.InputStream.Position = 0;
        //        HttpContext context = new HttpContext(request, response);
        //        new MysteryActionRouteHandler<PropertyEditAction, MonoPropertyContent>().ProcessRequest(context);
        //    }
        //    var json = output.ToString();
        //}
    }
}
