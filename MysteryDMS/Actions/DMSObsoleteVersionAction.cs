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
    public class DMSObsoleteVersionAction : BaseContentAction
    {
        public DMSObsoleteVersionAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label { get { return "DMS.VERSION.OBSOLETE"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "archive";
                result.style = "default obsolete-workflow-button";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_obsolete;
            version.next_actors.Clear();
            cd.Add(version);
            
            var result = new ContentActionOutput();
            result.changed_contents.Add(version);
            result.next_url = new ContentUrl(version);

            return result;
        }

        /// <summary>
        /// Author can always obsolete a version.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;

            if (version.workflow_type.value == null)
            {
            }
            else if (version.workflow_type.value.ToString() == DMSConstants.simple_wf)
            {
                if ((version.wf_Status == DMSConstants.wf_in_work && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_in_work && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_finalized && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_finalized && version.co_authors.Contains(user))) return true;
            }
            else if (version.workflow_type.value.ToString() == DMSConstants.complex_wf)
            {
                if((version.wf_Status == DMSConstants.wf_in_work && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_in_work && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_under_review && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_under_review && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_reviewed_OK && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_reviewed_OK && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_reviewed_KO && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_reviewed_KO && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_approved && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_approved && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_approved_w_comments && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_approved_w_comments && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_disapproved_w_comments && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_disapproved_w_comments && version.co_authors.Contains(user))) return true;
            }

            return false;
        }
    }
}
