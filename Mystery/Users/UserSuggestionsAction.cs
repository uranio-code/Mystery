using Mystery.MysteryAction;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;

namespace Mystery.Users
{

    public class UserSuggestionsAction : BaseMysteryAction<IEnumerable<LightContentReferece>>
    {
        protected override ActionResult<IEnumerable<LightContentReferece>> ActionImplemetation()
        {

            var cd = this.getGlobalObject<IContentDispatcher>();
            return new ActionResult<IEnumerable<LightContentReferece>>(
                cd.GetLightContentRereferece<User>());
            
        }

        protected override bool AuthorizeImplementation()
        {
            //any logged user
            return true;

        }
    }
}
