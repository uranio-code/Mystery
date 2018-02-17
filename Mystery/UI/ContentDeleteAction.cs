using Mystery.Content;
using System;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.Users;
using System.Collections.Generic;

namespace Mystery.UI
{

    

    public class ContentDeleteAction : BaseContentAction, IPublishedAction<WhoWhatWhen>
    {
        public ContentDeleteAction(IContent content, User user) : base(content, user)
        {
        }

        public override string name { get { return "delete"; } }
        public override string label { get { return "DELETE"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.style = "danger btn-outline";
                result.font_awesome_icon = "trash";
                result.font_awesome_icon_2 = "";
                return result;
            }
        }

        public override Modal modal{get{
                return new Modal{
                    controller = "deleteContentController as c",
                    templateUrl = "MysteryWebContent/MysteryContent/Actions/DeleteContent.html"
                };
            }}

        public string history_message_template_url
        {
            get
            {
                return "MysteryWebContent/History/ContentDeleted.html";
            }
        }

        public WhoWhatWhen history_message_data { get; private set; }

        public List<string> history_tags { get; private set; } = new List<string>();

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IContent content = cd.GetContent(input.getContenTypeName(),input.guid);
            if (content == null) return new ActionResult<ContentActionOutput>() { isSuccessfull = false, UnAuthorized = false, message = "content not found" };
            var result = new ContentActionOutput();
            result.deleted_contents.Add(content);
            result.message = "CONTENT_DELETED";
            result.next_url = new ContentTypeUrl(content.GetType());
            cd.Remove(content);
            //history
            history_message_data = new WhoWhatWhen() {
                who = user.guid,
                what = new ContentReference(content),
                when = DateTime.Now };
            history_tags.Add(nameof(ContentDeleteAction));
            history_tags.Add(content.getContenTypeName());
            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IContent content = cd.GetContent(input.getContenTypeName(),input.guid);
            if (content == null) return false;
            return content.GetType().getMysteryAttribute<ContentDeleteAttribute>().canDelete(content, user);
        }
    }
}
