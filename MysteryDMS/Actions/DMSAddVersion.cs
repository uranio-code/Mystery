using Mystery.Content;
using Mystery.Messaging;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using Mystery;
using MysteryDMS.Model;


namespace MysteryDMS.Actions
{
    public class DMSAddVersion : BaseContentAction
    {
        public DMSAddVersion(IContent content, User user) : base(content, user)
        {
        }

        public override string label{get { return "DMS.VERSION.ADDVERSION"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "file-o";
                result.font_awesome_icon_2 = "plus";
                result.style = "info btn-outline";
                return result;
            }
        }

        /// <summary>
        /// We create a new version and we'll inherit the properties excluding:
        /// original file
        /// cover page file
        /// version change description
        /// </summary>
        /// <returns></returns>
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();

            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            DMSVersion last_version = version;
            while (last_version.next_version.value != null) last_version = last_version.next_version;


            var cc = this.getGlobalObject<IGlobalContentCreator>();
            DMSVersion new_version = cc.getNewContent<DMSVersion>();

            //the creator of the version is the owner
            new_version.owner = user;

            //the version number shall be the number of the last version + 1
            new_version.version_number = last_version.version_number + 1;

            //the author is inherited from the last version. If it was empty, the author is the creator of the version
            if (version.author.value == null)
            {
                new_version.author = user;
            }
            else
            {
                new_version.author = version.author;
            }

            //the workflow type is inherited from the last version
            if (version.workflow_type.value != null)
            {
                new_version.workflow_type = version.workflow_type;
                //the workflow status depends on the workflow type
                if (version.workflow_type.value.ToString() == DMSConstants.simple_wf)
                {
                    new_version.wf_Status = DMSConstants.wf_in_work;
                }
                else if (version.workflow_type.value.ToString() == DMSConstants.complex_wf)
                {
                    new_version.wf_Status = DMSConstants.wf_in_work;
                }
            }

            new_version.signatories = version.signatories;
            new_version.co_authors = version.co_authors;
            new_version.approvers = version.approvers;
            new_version.title = version.title;
            new_version.description = version.description;            
            new_version.external_reference_id = version.external_reference_id;
            new_version.tags = version.tags;
            new_version.parent_folders = version.parent_folders;
            new_version.previous_version = last_version;
            new_version.allow_comments = version.allow_comments;
            new_version.comments_visible = version.comments_visible;
            cd.Add(new_version);
            result.new_contents.Add(new_version);
            result.message = "Document version created";
            
            foreach (DMSFolder folder in version.parent_folders)
            {
                folder.versions.Add(new_version);
                cd.Add(folder);
                result.changed_contents.Add(folder);
            }
            

            last_version.next_version = new_version;
            cd.Add(last_version);
            result.changed_contents.Add(last_version);

            INotifier notifier = this.getGlobalObject<INotifier>();

            IMessageManager mm = this.getGlobalObject<IMessageManager>();
            ICodifiedMessage cmessage = mm.getCodifiedMessage(DMSEnumerations.add_version_message_code, DMSEnumerations.language);
            IUserMessage message = cmessage.message;
            if (message != null)
            {
                message.body = message.body.Replace("#title", new_version.title);
                message.from = user;
                message.to = version.getSubscribers();
                notifier.sendMessage(message);

            }

            result.next_url = new ContentUrl(new_version);
            return result;
        }

        /// <summary>
        /// the owner of the version, the owner of any of the parent folders and the author can add a new version.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;

            foreach(DMSFolder folder in version.parent_folders)
            {
                if (folder.rowner.Equals(this.user)) return true; 
            }
            
            return (version.owner == this.user || version.author == this.user);

        }
    }
}
