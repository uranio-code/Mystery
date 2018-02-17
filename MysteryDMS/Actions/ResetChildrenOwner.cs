using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System.Collections.Generic;

namespace MysteryDMS.Actions
{
    class ResetChildrenOwner : BaseContentAction
    {
        public ResetChildrenOwner(IContent content, User user) : base(content, user)
        {
        }

        public override string label
        {
            get
            {
                return "DMS.RESETCHILDRENOWNER";
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var c = (IDMSContent)cd.GetContent(input.getContenTypeName(), this.input.guid);

            
            foreach (IDMSContent child in c.children)
            {
                child.owner = c.owner;
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
