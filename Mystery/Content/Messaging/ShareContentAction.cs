using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.UI;
using Mystery.Users;

namespace Mystery.Messaging
{

    public class ShareContentActionInput {
        public string content_tiny_guid { get; set; }
        public string content_type { get; set; }
        public List<string> receivers { get; set; } = new List<string>();
        public string email_body { get; set; }
        
    }

    [PublishedAction(input_type: typeof(ShareContentActionInput), url = nameof(ShareContentAction))]
    public class ShareContentAction : BaseMysteryAction<ShareContentActionInput, ContentActionOutput>, IPublishedAction<IUserMessage>
    {
        public IUserMessage history_message_data { get; private set; }
        

        public string history_message_template_url
        {
            get
            {
                return "MysteryWebContent/MysteryContent/Actions/ShareContentLog.html";
            }
        }


        public List<string> history_tags { get; private set; } = new List<string>();
        public bool has_history { get; private set; } = true;

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var cr = ContentReference.tryGetContentReferece(input.content_type, input.content_tiny_guid);
            if (cr == null)
                return ActionResultTemplates< ContentActionOutput>.InvalidInput;
            var content = cr.getContent();
            if (content == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var message = new BaseMessage();

            var users_reference = (from string x in input.receivers
                                   select ContentReference.tryGetContentReferece(nameof(User), x));
            var users = (from ContentReference x in users_reference
                         where x != null
                         select (User)x.getContent());
            users = (from User x in users where x != null select x);
            message.from = user;
            message.to = users;
            message.body = input.email_body;
            message.title = content.ReferenceText;

            history_message_data = message;
            history_tags.Add(nameof(ShareContentAction));
            history_tags.Add(content.getContenTypeName());
            history_tags.Add(content.guid.ToString());

            INotifier notifier = this.getGlobalObject<INotifier>();
            notifier.sendMessage(message);
            

            return ActionResultTemplates<ContentActionOutput>.ACK;

        }

        protected override bool AuthorizeImplementation()
        {
            var cr = ContentReference.tryGetContentReferece(input.content_type, input.content_tiny_guid);
            if (cr == null)
                return false;
            var c = cr.getContent();
            if (c == null)
                return false;
            return c.canAccess(user);
        }
    }
}
