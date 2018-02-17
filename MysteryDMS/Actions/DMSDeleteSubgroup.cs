
using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;

namespace MysteryDMS.Actions
{
    class DMSDeleteSubgroup : BaseContentAction
    {

        public DMSDeleteSubgroup(IContent content, User user) : base(content, user)
        {
        }

        public override string name
        {
            get
            {
                return nameof(DMSDeleteSubgroup);
            }
        }

        public override string label
        {
            get
            {
                return "DMS.DELETEUSERGROUP";
            }
        }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "file-o";
                result.font_awesome_icon_2 = "trash";
                result.style = "danger btn-outline";
                return result;
            }
        }

        /// <summary>
        /// This creates a new DMSFolder and redirects to its view page.
        /// </summary>
        /// <returns></returns>
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSUserGroup group = cd.GetContent<DMSUserGroup>(this.input.guid);
            DMSUserGroup pointer = group;

            DMSUserGroup parent = group.parent_group;
            if(parent != null)
            {
                parent.sub_groups.Remove(group);
                cd.Add(parent);
            }

            group.deleteSubgroups();
                        
            return result;
        }

        /// <summary>
        /// DMS-84
        /// Groups shall only be deleted by admin or the user that created them.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {

            if (user.account_type == UserType.admin) return true;

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSUserGroup group = cd.GetContent<DMSUserGroup>(this.input.guid);
            if (group == null) return false;

            if (user.guid == group.creator.guid) return true;

            return false;
        }
    }


    
}
