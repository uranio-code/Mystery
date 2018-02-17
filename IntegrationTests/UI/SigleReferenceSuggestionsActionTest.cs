using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Json;
using Mystery.Register;
using Mystery.UI;
using Mystery.Users;
using Mystery.Web;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.UI
{
    [TestClass()]
    public class SigleReferenceSuggestionsActionTest
    {
        [TestMethod()]
        public void SigleReferenceSuggestionsActionCanTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                TestContentType adriano = TestContentType.getARandomTestContentType(enforce_a_reference: true);
                ContentsDatabase db = this.getGlobalObject<ContentsDatabase>();
                db.Add(adriano);

                var result = executor.executeAction(
                    new SigleReferenceSuggestionsAction(),
                    new PropertyEditSuggestionsActionInput {
                        content_reference = new Mystery.Content.ContentReference(adriano),
                        property_name =  nameof(TestContentType.single_reference),
                        search_text = null
                    });
                Assert.IsTrue(result.isSuccessfull);

            }
        }
        [TestMethod()]
        public void SigleReferenceSuggestionsActionCanNotTest()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                //we add Adriano in db
                TestContentType adriano = TestContentType.getARandomTestContentType(enforce_a_reference: true);
                ContentsDatabase db = this.getGlobalObject<ContentsDatabase>();
                db.Add(adriano);
                //we became not admin
                MysterySession session = this.getGlobalObject<MysterySession>();
                var was_adim = session.authenticated_user.account_type == UserType.admin;
                if (was_adim) {
                    session.authenticated_user.account_type = UserType.normal;
                }

                var result = executor.executeAction(
                    new SigleReferenceSuggestionsAction(),
                    new PropertyEditSuggestionsActionInput
                    {
                        content_reference = new Mystery.Content.ContentReference(adriano),
                        property_name = nameof(TestContentType.not_ediatable_reference),
                        search_text = null
                    });
                Assert.IsTrue(!result.isSuccessfull && result.UnAuthorized);
                if (was_adim)
                {
                    session.authenticated_user.account_type = UserType.admin;
                }

            }
        }
    }
}
