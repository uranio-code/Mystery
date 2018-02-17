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
    class DMSDeleteFolder : BaseContentAction
    {
        public DMSDeleteFolder(IContent content, User user) : base(content, user)
        {
        }

        public override string name
        {
            get
            {
                return "delete";
            }
        }

        public override string label
        {
            get
            {
                return "DMS.FOLDER.DELETEFOLDER";
            }
        }

        /// <summary>
        /// deleted the current folder and obsolete all the content inside it
        /// </summary>
        /// <returns></returns>
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSFolder folder = cd.GetContent<DMSFolder>(this.input.guid);
            foreach (DMSVersion version in folder.versions)
            {
                version.status = DMSConstants.obsolete;
                cd.Add(version);
                result.changed_contents.Add(version);
            }
            //as we are going recursive the subfolder collection is going to change
            var local_copy_for_loop = new List<DMSFolder>(folder.subfolders.getEnum());
            foreach (DMSFolder sub_folder in local_copy_for_loop) {
                if (sub_folder == null)
                    continue;
                var sub_deleted_result = this.executeAction(new DMSDeleteFolder(sub_folder, user));
                result.changed_contents.AddRange(sub_deleted_result.changed_contents);
                result.deleted_contents.AddRange(sub_deleted_result.deleted_contents);
                result.new_contents.AddRange(sub_deleted_result.new_contents);
            }
            cd.Remove(folder);
            result.deleted_contents.Add(folder);

            foreach (DMSFolder parent in folder.parent_folders) {
                if (parent == null)
                    continue;
                parent.subfolders.Remove(folder);
                result.changed_contents.Add(parent);
            }

            result.message = "DMS.FOLDER.FOLDER_DELETED";


            result.next_url = new ContentUrl(folder.parent_folders?.FirstOrDefault()?.getContent());
            return result;
        }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "folder-o";
                result.font_awesome_icon_2 = "trash";
                result.style = "danger btn-outline";
                return result;
            }
        }

        protected override bool AuthorizeImplementation()
        {
            if (this.user.account_type == UserType.admin)
                return true;
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSFolder folder = cd.GetContent<DMSFolder>(this.input.guid);
            if (folder == null) { return false; }

            return folder.rowner.Equals(this.user);
        }
    }
}
