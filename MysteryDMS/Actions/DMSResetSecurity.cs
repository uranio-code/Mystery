using System;
using Mystery.Content;
using Mystery.Security;
using Mystery.UI;
using Mystery.Users;
using Mystery.Register;
using MysteryDMS.Model;

namespace MysteryDMS.Actions
{
    
    /// <summary>
    /// It resets the security of this folder taking the one from the parent.
    /// </summary>
    public class DMSResetSecurity : BaseContentAction
    {
        public DMSResetSecurity(IContent content, User user) : base(content, user)
        {
        }

        public override string label
        {
            get
            {
                return "DMS.RESETSECURITYTOMATCHPARENT";
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var c = cd.GetContent<IDMSContent>(this.input.guid);

            c.add_permission_users = c.parent_dms_content.add_permission_users;
            c.view_permission_users = c.parent_dms_content.view_permission_users;
            c.edit_permission_users = c.parent_dms_content.edit_permission_users;

            var result = new ContentActionOutput();
            result.next_url = new ContentUrl(c);

            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var c = cd.GetContent<IDMSContent>(this.input.guid);
            if (c == null) { return false; }
            if (c.parent_dms_content == null) { return false; }
            return c.owner.Equals(this.user);

        }
    }
}
