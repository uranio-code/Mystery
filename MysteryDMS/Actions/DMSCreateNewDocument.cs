using Mystery.UI;
using System;
using Mystery.Content;
using Mystery.Users;
using Mystery.MysteryAction;
using Mystery.Register;
using MysteryDMS.Model;
using System.Linq;
using System.Collections.Generic;

namespace MysteryDMS.Actions
{
    public class DMSCreateNewDocument : BaseContentAction, IPublishedAction<WhoWhatWhen>
    {
        public DMSCreateNewDocument(IContent content, User user) : base(content, user)
        {
        }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "file-o";
                result.font_awesome_icon_2 = "plus";
                result.style = "primary btn-outline";
                return result;
            }
        }

        public WhoWhatWhen history_message_data { get; private set; }
        

        public string history_message_template_url
        {
            get
            {
                return "DMS/Version/VersionCreatedHistoryEntry.html";
            }
        }

        public List<string> history_tags { get; private set; } = new List<string>();
        

        public override string label
        {
            get
            {
                return "DMS.FOLDER.ADDDOCUMENT";
            }
        }
        public bool has_history { get; private set; } = true;

        /// <summary>
        /// This creates a new DMSVersion and redirects to its view page.
        /// </summary>
        /// <returns></returns>
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSFolder folder = cd.GetContent<DMSFolder>(this.input.guid);

            var cc = (new object()).getGlobalObject<IGlobalContentCreator>();
            DMSVersion new_version = cc.getNewContent<DMSVersion>();

            
            new_version.title = "New document title";
            new_version.parent_folders.Add( folder);
            new_version.status = DMSConstants.current;
            new_version.author = user;

            new_version.view_permission_users = folder.view_permission_users;
            new_version.view_permission_groups = folder.view_permission_groups;

            new_version.edit_permission_users = folder.edit_permission_users;
            new_version.edit_permission_groups = folder.edit_permission_groups;

            new_version.add_permission_users = folder.add_permission_users;
            new_version.add_permission_groups = folder.add_permission_groups;

            result.message = "Document version created";

            cd.Add(new_version);
            result.next_url = new ContentUrl(new_version);
            result.new_contents.Add(new_version);

            folder.versions.Add(new_version);
            cd.Add(folder);
            result.changed_contents.Add(folder);
            result.main = new_version;
            history_message_data = new WhoWhatWhen() {
                who = user.guid,
                what = new ContentReference(new_version),
                when = DateTime.Now };
            history_tags.Add(nameof(DMSCreateNewDocument));
            history_tags.Add(new_version.getContenTypeName());
            return result;
        }

        /// <summary>
        /// S-DMS-REQ-35
        /// The S-DMS shall allow Users to create, edit and delete any content inside the “Training” folder.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSFolder folder = cd.GetContent<DMSFolder>(this.input.guid);
            if (folder == null){ return false; }

            if (folder.GetType() == typeof(DMSTrainingFolder)) { return true; }

            if (((IDMSContent)folder).owner == this.user) { return true; }

            MultiContentReference<User> add_permission = folder.add_permission;
            if (add_permission == null) { return false; }

            return add_permission.Contains(user);
        }
    }
}
