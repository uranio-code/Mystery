using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System.Collections.Generic;
using System.Linq;
using Mystery.Web;
using System;

namespace MysteryDMS.Actions
{
    class DMSCreateNewFolder : BaseContentAction
    {

        public DMSCreateNewFolder(IContent content, User user) : base(content, user)
        {
        }

        public override string name
        {
            get
            {
                return nameof(DMSCreateNewFolder);
            }
        }

        public override string label
        {
            get
            {
                return "DMS.FOLDER.ADDFOLDER";
            }
        }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "folder-o";
                result.font_awesome_icon_2 = "plus";
                result.style = "success btn-outline";
                return result;
            }
        }

        /// <summary>
        /// This creates a new DMSFolder and redirects to its view page.
        /// </summary>
        /// <returns></returns>
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSFolder folder = cd.GetContent<DMSFolder>(this.input.guid);

            var cc = (new object()).getGlobalObject<IGlobalContentCreator>();
            DMSFolder new_folder = cc.getNewContent<DMSFolder>();
            
            new_folder.title = "DMS.NEWFOLDERTITLE";
            
            new_folder.parent_folders.Add(folder);
            folder.subfolders.Add(new_folder);

            // add the users
            new_folder.add_permission_users = folder.add_permission_users;
            new_folder.add_permission_groups = folder.add_permission_groups;

            new_folder.view_permission_users = folder.view_permission_users;
            new_folder.view_permission_groups = folder.view_permission_groups;

            new_folder.edit_permission_users = folder.edit_permission_users;
            new_folder.edit_permission_groups = folder.edit_permission_groups;
            
            new_folder.rowner = this.user;
            cd.Add(new_folder);

            result.message = "DMS.MESSAGES.NEWFOLDERCREATED";
            result.changed_contents.Add(folder);
            result.new_contents.Add(new_folder);
            result.next_url = new ContentUrl(new_folder);
            result.main = new_folder;
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
            if (folder == null) { return false; }
            if (folder.GetType() == typeof(DMSTrainingFolder)) { return true; }
            MultiContentReference<User> add_permission = folder.add_permission;
            if (add_permission == null) { return false; }

            if (folder.rowner == this.user) { return true; }

            return add_permission.Contains(user);
        }
    }


    public class DMSCreateNewFolderFromTreeInput :  ContentInput {
        public string new_folder_title { get; set; }
    }

    [PublishedAction(input_type: typeof(DMSCreateNewFolderFromTreeInput))]
    public class DMSCreateNewFolderFromTree : BaseMysteryAction<DMSCreateNewFolderFromTreeInput, ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var parent_folder = cd.GetContent<DMSFolder>(input.tiny_guid.fromTiny());
            if (parent_folder == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;
            var base_result = this.executeAction(new DMSCreateNewFolder(parent_folder, user));
            if (base_result == null || base_result.main == null)
                return base_result;
            var new_folder = (DMSFolder)base_result.main;
            new_folder.title = input.new_folder_title;
            cd.Add(new_folder);
            //the folder is already in the new contents
            return base_result;
        }

        protected override bool AuthorizeImplementation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var parent_folder  = cd.GetContent<DMSFolder>(input.tiny_guid.fromTiny());
            if (parent_folder == null)
                return false;
            //we link it to DMSCreateNewFolder action to handle cases only in 1 place
            var action = new DMSCreateNewFolder(parent_folder, user);
            return action.Authorize();
            //var info_input = new ContentGetActionInfoActionInput();
            //info_input.content_reference = new ContentReference(parent_folder);
            //info_input.action_name = action.name;
            //var action_info = this.executeAction(new ContentGetActionInfoAction(), info_input);
            //return (action_info != null && action_info.button.enabled);
        }
    }
}
