using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using System;
using MysteryDMS.Model;
using Mystery.Users;
using MysteryWebLogic.Authetication;
using Mystery.UI;
using Mystery.Comment;

namespace MysteryDMS.Actions
{
    public class DMSSignAction : BaseContentAction
    {
        public DMSSignAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label{get{ return "DMS.VERSION.SIGN"; }}

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
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_signed;
            if (version.active) version.next_actors = version.approvers;
            cd.Add(version);

            // use the signature API
            //throw new NotImplementedException();

            var result = new ContentActionOutput();
            result.changed_contents.Add(version);
            result.next_url = new ContentUrl(version);

            return result;
        }


        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;
            if (version.wf_Status != DMSConstants.wf_in_work) return false;


            User actor = version.author;
            if (actor != null && actor.Equals(this.user)) return true;
            
            foreach (User factor in version.co_authors)
            {
                if (factor != null && factor.Equals(this.user)) return true;
            }

            return false;

        }
    }
}
