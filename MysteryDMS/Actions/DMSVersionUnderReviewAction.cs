using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System.Collections.Generic;
using System.Linq;

namespace MysteryDMS.Actions
{
    public class DMSVersionUnderReviewAction : BaseContentAction
    {
        public DMSVersionUnderReviewAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label { get { return "DMS.VERSION.UNDER_REVIEW"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "gears";
                result.style = "primary";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_under_review;


            if (version.next_version == null && version.previous_version == null)
            {
                // this is the only one. Easy case.
                version.status = DMSConstants.current;
            }
            else
            {
                if (version.previous_version == null)
                {
                    // implement the logic
                }
            }


            cd.Add(version);

            // use the signature API
            //throw new NotImplementedException();

            var result = new ContentActionOutput();
            result.changed_contents.Add(version);
            result.next_url = new ContentUrl(version);

            return result;
        }

        /// <summary>
        /// Under review is available to reviewers for under control documents.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;

            ///////////////////// Complex Workflow
            // In work
            if ((version.wf_Status == DMSConstants.wf_in_work && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_in_work && version.co_authors.Contains(user))) return true;

            // Reviewed OK
            if ((version.wf_Status == DMSConstants.wf_reviewed_OK && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_reviewed_OK && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_reviewed_OK && version.reviewers.Contains(user))) return true;

            // Reviewed KO
            if ((version.wf_Status == DMSConstants.wf_reviewed_KO && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_reviewed_KO && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_reviewed_KO && version.reviewers.Contains(user))) return true;

            return false;
        }
    }
}
