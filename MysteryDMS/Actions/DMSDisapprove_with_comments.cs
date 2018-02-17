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
using Mystery.Web;

namespace MysteryDMS.Actions
{

    [PublishedAction(input_type: typeof(AddCommentInput), url = nameof(DMSDisapprove_with_comments))]
    class DMSDisapprove_with_comments : BaseMysteryAction<AddCommentInput, ContentActionOutput>, IContentActionButton
    {

        public DMSDisapprove_with_comments() : base()
        {
        }

        public DMSDisapprove_with_comments(DMSVersion version, User user)
        {
            this.input = new AddCommentInput { content = version };
            this.user = user;
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = this.input.content;

            var cc = (new object()).getGlobalObject<IGlobalContentCreator>();
            DMSComment new_comment = cc.getNewContent<DMSComment>();

            new_comment.owner = this.user;
            new_comment.comment_body = this.input.comment_text;
            new_comment.add_permission_users = version.add_permission_users;
            new_comment.add_permission_groups = version.add_permission_groups;
            new_comment.view_permission_users = version.view_permission_users;
            new_comment.view_permission_groups = version.view_permission_groups;
            new_comment.edit_permission_users = version.edit_permission_users;
            new_comment.edit_permission_groups = version.edit_permission_groups;
            new_comment.parent_dms_content = version;
            version.wf_Status = DMSConstants.wf_disapproved_w_comments;
            version.next_actors.Clear();
            cd.Add(version);
            cd.Add(new_comment);


            var result = new ContentActionOutput();
            result.changed_contents.Add(version);
            result.new_contents.Add(new_comment);
            result.next_url = new ContentUrl(version);

            return result;
        }

        public Button button
        {
            get
            {
                var result = new Button();
                result.enabled = this.Authorize();
                result.label = this.label;
                result.font_awesome_icon = "times";
                result.style = "danger";
                return result;
            }
        }

        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = this.input.content;
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

        
        public string label { get { return "DMS.VERSION.DISAPPROVE_WITH_COMMENT"; } }

        public Modal modal
        {
            get
            {
                return new Modal()
                {
                    controller = "dms_disapprove_w_comments_controller as c",
                    templateUrl = "DMS/add_comment_modal.html",
                };
            }
        }

        public string name
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }
}
