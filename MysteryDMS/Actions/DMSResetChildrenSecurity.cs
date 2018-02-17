using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System.Collections.Generic;

namespace MysteryDMS.Actions
{
    public class DMSResetChildrenSecurity : BaseContentAction
    {
        public DMSResetChildrenSecurity(IContent content, User user) : base(content, user)
        {
        }

        public override string label
        {
            get
            {
                return "DMS.RESETCHILDRENSECURITY";
            }
        }

        public override Button button
        {
            get
            {
                var result = base.button;
                // result.font_awesome_icon = "your owesome icon";
                // result.style = "your own style";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IDMSContent c = (IDMSContent)cd.GetContent(input.getContenTypeName(), this.input.guid);

            foreach (IDMSContent child in c.children)
            {
                child.add_permission_users = c.add_permission_users;
                child.add_permission_groups = c.add_permission_groups;
                child.edit_permission_users = c.edit_permission_users;
                child.edit_permission_groups = c.edit_permission_groups;
                child.view_permission_users = c.view_permission_users;
                child.view_permission_groups = c.view_permission_groups;
            }

            var result = new ContentActionOutput();
            result.next_url = new ContentUrl(c);

            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var c = (IDMSContent)cd.GetContent(input.getContenTypeName(), this.input.guid);
            if (c == null) { return false; }
            
            return c.owner.Equals(this.user);

        }
    }
}
