using Mystery.AccessHistory;
using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Mystery.History;
using Mystery.Users;

namespace Users
{

    public class UserActivityStream :
        BaseMysteryAction<User,IEnumerable<IPublishedAction>>
    {

        
        protected override ActionResult<IEnumerable<IPublishedAction>> ActionImplemetation()
        {
            var history = this.getGlobalObject<IHistoryRepository>();

            var recent= history.GetByUser(input,max_result:20,min_date:DateTime.Now.AddMonths(-1));

            return new ActionResult<IEnumerable<IPublishedAction>>(recent);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
