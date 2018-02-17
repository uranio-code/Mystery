using Mystery.Content;
using Mystery.Messaging;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using Mystery.Web;
using MysteryDMS.Model;
using System.Linq;

namespace MysteryDMS.Actions
{


    [PublishedAction(input_type: typeof(AddCommentInput), url = nameof(DMSAddCommentAction))]
    public class DMSAddCommentAction : BaseMysteryAction<AddCommentInput, ContentActionOutput>, IContentActionButton
    {

        /// <summary>
        /// constructor invoked when the action is triggered from web
        /// </summary>
        public DMSAddCommentAction() : base()
        {
        }
        /// <summary>
        /// constructor invoked when the the action is request as IContentActionButton
        /// </summary>
        /// <param name="content"></param>
        /// <param name="user"></param>
        public DMSAddCommentAction(DMSVersion version, User user) :  base()
        {
            this.input = new AddCommentInput { content = version };
            this.user = user;
        }

        public string label { get { return "DMS.VERSION.ADD_COMMENT"; } }

        public Button button
        {
            get
            {
                var result = new Button();
                result.enabled = this.Authorize();
                result.label = this.label;
                // result.font_awesome_icon = "your owesome icon";
                // result.style = "your own style";
                return result;
            }
        }

        public Modal modal
        {
            get
            {
                return new Modal()
                {
                    controller = "dms_add_comment_modal_controller as c",
                    templateUrl = "DMS/add_comment_modal.html",
                };
            }
        }

        public string name
        {
            get
            {
                return nameof(DMSAddCommentAction);
            }
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
            cd.Add(new_comment);

            var result = new ContentActionOutput();
            result.changed_contents.Add(version);
            result.new_contents.Add(new_comment);
            result.next_url = new ContentUrl(version);

            INotifier notifier = this.getGlobalObject<INotifier>();
            IMessageManager mm = this.getGlobalObject<IMessageManager>();
            ICodifiedMessage cmessage = mm.getCodifiedMessage(DMSEnumerations.add_comment_message_code, DMSEnumerations.language);
            IUserMessage message = cmessage.message;
            if (message != null)
            {
                message.body = message.body.Replace("#comment_text", new_comment.comment_body);

                message.from = user;
                message.to = version.getSubscribers();
                notifier.sendMessage(message);

            }

            return result;
        }

        /// <summary>
        /// S-DMS-REQ-59 
        /// The S-DMS shall allow Users to add comments to contents as long as: 
        /// the User has Add permission on that content, AND
        /// the Owner of the content allows Users to create comments on that content.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            DMSVersion version = this.input.content;
            if (version == null) return false;
            
            if (!version.allow_comments) return false;

            if (user.account_type == UserType.admin) return true;

            if (version.add_permission == null) return false;

            return version.add_permission.Contains(user);

        }
    }
}
