using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;

namespace MysteryDMS.Actions
{
    class DMSCreateSubgroup : BaseContentAction
    {

        public DMSCreateSubgroup(IContent content, User user) : base(content, user)
        {
        }

        public override string name
        {
            get
            {
                return nameof(DMSCreateSubgroup);
            }
        }

        public override string label
        {
            get
            {
                return "DMS.ADDUSERGROUP";
            }
        }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "folder-o";
                result.font_awesome_icon_2 = "plus";
                result.style = "success btn-outline";
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
            DMSUserGroup parent = cd.GetContent<DMSUserGroup>(this.input.guid);

            var cc = (new object()).getGlobalObject<IGlobalContentCreator>();
            DMSUserGroup new_group = cc.getNewContent<DMSUserGroup>();

            new_group.title = "DMS.NEWUSERGROUPTITLE";
            new_group.creator = user;

            new_group.parent_group = parent;
            parent.sub_groups.Add(new_group);

            // add the users
            new_group.add_permission_users = parent.add_permission_users;
            new_group.add_permission_groups = parent.add_permission_groups;

            new_group.view_permission_users = parent.view_permission_users;
            new_group.view_permission_groups = parent.view_permission_groups;

            new_group.edit_permission_users = parent.edit_permission_users;
            new_group.edit_permission_groups = parent.edit_permission_groups;

            cd.Add(new_group);
            cd.Add(parent);

            result.message = "DMS.MESSAGES.NEWUSERGROUPCREATED";
            result.changed_contents.Add(parent);
            result.new_contents.Add(new_group);
            result.next_url = new ContentUrl(new_group);
            result.main = new_group;
            return result;
        }

        /// <summary>
        /// S-DMS-REQ-35
        /// The S-DMS shall allow Users to create, edit and delete any content inside the “Training” folder.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {

            if (user.account_type == UserType.admin) return true;
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSUserGroup parent = cd.GetContent<DMSUserGroup>(this.input.guid);
            if (parent == null) { return false; }
            
            MultiContentReference<User> add_permission = parent.add_permission;
            if (add_permission == null) { return false; }
                      

            return add_permission.Contains(user);
        }
    }


    
}
