using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Register;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Content
{
    [TestClass()]
    public class SingleReferencePropertyValuesProviderAttTests
    {
        

        [TestMethod()]
        public void SingleReferencePropertyValuesProviderAttgetProviderTest()
        {

            var cd = this.getGlobalObject<IContentDispatcher>();
            cd.Add(TestContentType.getARandomTestContentTypeWithoutreference());

            //let's try to just call it
            foreach (var provider_att in typeof(TestContentType).getMysteryPropertyAttributes<SingleReferencePropertyValuesProviderAtt>())
            {
                var result = provider_att.getProvider().getSuggestions(null, "");
                Assert.IsTrue(result.Count() > 0);
            }

           
        }
    }
}