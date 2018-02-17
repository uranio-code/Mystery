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
    public class DMSInWorkAction : BaseContentAction
    {
        public DMSInWorkAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label{get{ return "DMS.VERSION.IN_WORK"; }}

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "wrench";
                result.style = "default in-work-workflow-button";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_in_work;
            //if (version.active) version.next_actors = version.approvers;
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

            if(version.workflow_type.value == null)
            {
            }
            else if (version.workflow_type.value.ToString() == DMSConstants.simple_wf)
            {
                if ((version.wf_Status == DMSConstants.wf_finalized && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_finalized && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_obsolete && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_obsolete && version.co_authors.Contains(user)))
                {
                    return true;
                } 
                else if ((version.wf_Status == DMSConstants.wf_in_work && version.author == user) ||
                        (version.wf_Status == DMSConstants.wf_in_work && version.co_authors.Contains(user)))
                {
                    return false;
                }
                //This case is added in case the workflow status corresponds to a different workflow, and the workflow type is changed into "Simple workflow"
                else if (version.author == user || version.co_authors.Contains(user))
                {
                    return true;
                }

            }
            else if (version.workflow_type.value.ToString() == DMSConstants.complex_wf)
            {
                if ((version.wf_Status == DMSConstants.wf_under_review && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_under_review && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_reviewed_OK && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_reviewed_OK && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_reviewed_KO && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_reviewed_KO && version.co_authors.Contains(user)) ||
                (version.wf_Status == DMSConstants.wf_obsolete && version.author == user) ||
                (version.wf_Status == DMSConstants.wf_obsolete && version.co_authors.Contains(user)))
                {
                    return true;
                }
                else if ((version.wf_Status == DMSConstants.wf_in_work && version.author == user) || 
                        (version.wf_Status == DMSConstants.wf_in_work && version.co_authors.Contains(user)) ||
                        (version.wf_Status == DMSConstants.wf_approved && version.author == user) ||
                        (version.wf_Status == DMSConstants.wf_approved && version.co_authors.Contains(user)) ||
                        (version.wf_Status == DMSConstants.wf_approved_w_comments && version.author == user) ||
                        (version.wf_Status == DMSConstants.wf_approved_w_comments && version.co_authors.Contains(user)) ||
                        (version.wf_Status == DMSConstants.wf_disapproved_w_comments && version.author == user) ||
                        (version.wf_Status == DMSConstants.wf_disapproved_w_comments && version.co_authors.Contains(user)))
                {
                    return false;
                }
                //This case is added in case the workflow status corresponds to a different workflow, and the workflow type is changed into "Complex workflow"
                else if (version.author == user || version.co_authors.Contains(user))
                {
                    return true;
                }
            }

                return false;

        }
    }
}
