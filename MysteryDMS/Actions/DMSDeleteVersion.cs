using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;

namespace MysteryDMS.Actions
{
    public class DMSDeleteVersion : BaseContentAction
    {
        public DMSDeleteVersion(IContent content, User user) : base(content, user)
        {
        }

        public override string label { get { return "DMS.VERSION.DELETEVERSION"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "trash";
                result.font_awesome_icon_2 = "";
                result.style = "danger btn-outline";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            version.status = DMSConstants.obsolete;
            version.next_actors.Clear();
            
            cd.Add(version);
            result.next_url = new ContentUrl(version);
            return result;
        }

        
        protected override bool AuthorizeImplementation()
        {
            if (user.account_type == UserType.admin)
                return true;
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;
            
            foreach (DMSFolder folder in version.parent_folders)
            {
                if (folder == null || folder.rowner.isNullOrEmpty())
                    continue;
                if (folder.rowner.Equals(this.user)) { return true; }
            }

            User author = version.author;
            if (author == null) return false;

            return author.Equals(this.user);

        }
    }
}