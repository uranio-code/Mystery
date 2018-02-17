using Mystery.Content;
using Mystery.Files;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Actions
{

    public class DMSCreateNewDocumentFromFileInput {
        public MysteryFile file { get; set; }
        public string tiny_folder_guid { get; set; }
    }

    [PublishedAction(input_type: typeof(DMSCreateNewDocumentFromFileInput), url = nameof(DMSCreateNewDocumentFromFile))]
    public class DMSCreateNewDocumentFromFile : BaseMysteryAction<DMSCreateNewDocumentFromFileInput, ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            DMSFolder folder = (DMSFolder)ContentReference.tryGetContentReferece(nameof(DMSFolder),  input.tiny_folder_guid).getContent();
            // we link it to DMSCreateNewDocument action to handle cases only in 1 place
            var action = new DMSCreateNewDocument(folder, user);
            var base_result = this.executeAction(action);
            var version = (DMSVersion)base_result.main;
            if (version == null)//something went wrong
                return base_result;
            version.original_file = input.file;
            version.title = input.file.filename;
            version.author = user;
            return base_result;
        }

        

        protected override bool AuthorizeImplementation()
        {
            var cr = ContentReference.tryGetContentReferece(nameof(DMSFolder), input.tiny_folder_guid);
            if (cr == null)
                return false;
            DMSFolder folder = (DMSFolder)cr.getContent();
            //we link it to DMSCreateNewDocument action to handle cases only in 1 place
            var action = new DMSCreateNewDocument(folder, user);
            var info_input = new ContentGetActionInfoActionInput();
            info_input.content_reference = cr;
            info_input.action_name = action.name;
            var action_info = this.executeAction(new ContentGetActionInfoAction(), info_input);
            return (action_info != null && action_info.button.enabled);
        }
    }
}
