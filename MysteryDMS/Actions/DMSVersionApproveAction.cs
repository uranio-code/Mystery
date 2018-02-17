using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Actions
{
    public class DMSVersionApproveAction : BaseContentAction
    {
        public DMSVersionApproveAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label { get { return "DMS.VERSION.APPROVE"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "check";
                result.style = "success";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_approved;
            version.next_actors.Clear();
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
            //Under review
            if (version.wf_Status == DMSConstants.wf_under_review && version.approvers.Contains(user)) return true;

            //Reviewed OK
            if (version.wf_Status == DMSConstants.wf_reviewed_OK && version.approvers.Contains(user)) return true;

            //Reviewed KO
            if (version.wf_Status == DMSConstants.wf_reviewed_KO && version.approvers.Contains(user)) return true;

            return false;

        }
    }
}
