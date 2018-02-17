using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System.Linq;
using Mystery.Register;
using Mystery.Content;
using MysteryDMS.Model;
using Mystery.Users;

namespace MysteryDMS.Actions
{
    public class AddUserGroupInput {
        public string fullname { get; set; }
    }

    [PublishedAction(input_type: typeof(AddUserGroupInput), url = nameof(DMSAddUserGroup))]
    public class DMSAddUserGroup : BaseMysteryAction<AddUserGroupInput, ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            if (string.IsNullOrWhiteSpace(input.fullname))
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var existing = cd.GetAllByFilter<DMSUserGroup>(x => x.title == input.fullname).FirstOrDefault();
            if (existing != null)
                return new ActionResult<ContentActionOutput>()
                {
                    isSuccessfull = false,
                    message = "USER.ALREADY_EXISTING",
                    output = new ContentActionOutput() { contents = {existing } }
                };
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var group = cc.getNewContent<DMSUserGroup>();
            group.title = input.fullname;
            cd.Add(group);

            return new ActionResult<ContentActionOutput>(new ContentActionOutput()
            {
                new_contents = { group },
                next_url = new ContentUrl(group)
            });
            
        }

        protected override bool AuthorizeImplementation()
        {
            //return user.account_type == UserType.admin;
            //All users can create groups
            return true;
        }
    }
}
