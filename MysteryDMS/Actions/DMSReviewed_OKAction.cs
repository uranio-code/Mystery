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
    public class DMSReviewed_OKAction : BaseContentAction
    {
        public DMSReviewed_OKAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label{get{ return "DMS.VERSION.REVIEWED_OK"; }}

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "thumbs-up";
                result.style = "info";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_reviewed_OK;
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

            ///////////////////// Complex Workflow
            // Under review
            if (version.wf_Status == DMSConstants.wf_under_review && version.reviewers.Contains(user)) return true;

            // Reviewed KO
            if (version.wf_Status == DMSConstants.wf_reviewed_KO && version.reviewers.Contains(user)) return true;

            return false;

        }
    }
}
