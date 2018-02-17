using Mystery.AccessHistory;
using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Mystery.History;

namespace MysteryDMS.StartPage
{
    [PublishedAction(input_type: null, url = nameof(GetLastDMSVersions))]
    public class GetLastDMSVersions :
        BaseMysteryAction<ContentActionOutput>
    {

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var access_register = this.getGlobalObject<UserAccessHistory>();
            var session = this.getGlobalObject<MysterySession>();
            var autheticated_users = session.authenticated_user;
            if (authenticated_user == null)
                return null;
            var last_versions_records = access_register.collection.Find(
                x => x.content_type_name == typeof(DMSVersion).getMysteryAttribute<ContentType>().name
                &&
                x.user_guid == authenticated_user.guid)
                .SortByDescending(x => x.entry_date)
                .Limit(20).ToList();
            var last_version_guid = (from x in last_versions_records select x.content_guid).ToList();
            var cd = this.getGlobalObject<IContentDispatcher>();
            return new ContentActionOutput()
            {
                contents = new List<IContent>(
                    cd.GetAllByFilter<DMSVersion>(
                        x => last_version_guid.Contains(x.guid))),
            };
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }

    public class GetLastDMSActions :
        BaseMysteryAction<IEnumerable<IPublishedAction>>
    {

        static List<string> tags = new List<string> {
            nameof(DMSVersion),
            nameof(DMSFolder),
        };

        protected override ActionResult<IEnumerable<IPublishedAction>> ActionImplemetation()
        {
            var history = this.getGlobalObject<IHistoryRepository>();

            var recented_dms = history.GetByTags(tags,max_result:20);

            return new ActionResult<IEnumerable<IPublishedAction>>(recented_dms);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
