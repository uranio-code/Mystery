using Mystery.Content;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System.Collections.Generic;

namespace MysteryDMS.Actions
{
    class DMSVersionWorkflowActionProvider : IContentButtonProvider
    {
        private IContentButtonProvider _default = new DefaultContentWorkflowActionMenuProvider<DMSVersion>();

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {

            var result = new List<IContentActionButton>();
            if (!(content is DMSVersion))
                return new List<IContentActionButton>();
            var version = (DMSVersion)content;

            ContentReference<DMSWorkflowType> reference = version.workflow_type;
            if (reference == null) return result;

            DMSWorkflowType workflow = reference.value;
            if (workflow == null)
            {
                result.Add(new DMSRemoveWorkflowStatusAction(version, user));
            return result;
            }
            // the workflow type defines the actions.
            // TBD: Find the way to transsform the switch to: 
            // result.AddRange(workflow_type.getActions(version, user);
            switch (version.workflow_type.value.title)
            {
                
                case DMSConstants.simple_wf:
                    result.Add(new DMSInWorkAction(version, user));
                    result.Add(new DMSFinalizeAction(version, user));
                    result.Add(new DMSObsoleteVersionAction(version, user));
                    break;

                case DMSConstants.complex_wf:
                    result.Add(new DMSInWorkAction(version, user));
                    result.Add(new DMSVersionUnderReviewAction(version, user));
                    result.Add(new DMSReviewed_OKAction(version, user));
                    result.Add(new DMSReviewed_KOAction(version, user));
                    result.Add(new DMSVersionApproveAction(version, user));
                    result.Add(new DMSApproveWithCommentsAction(version, user));
                    result.Add(new DMSDisapprove_with_comments(version, user));
                    result.Add(new DMSObsoleteVersionAction(version, user));
                    break;
            }
            
            return result;
        }
    }
}
