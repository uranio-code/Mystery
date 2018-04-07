using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Log
{
    [PublishedAction(input_type: null, url = nameof(RaiseError))]
    public class RaiseError : BaseMysteryAction<ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            //testing error logs
            throw new NotImplementedException();
        }

        protected override bool AuthorizeImplementation()
        {
            return user?.account_type == Users.UserType.admin; 
        }
    }
}
