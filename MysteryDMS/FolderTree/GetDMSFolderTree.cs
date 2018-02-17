using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using MysteryDMS.Model;
using System.Collections.Generic;

namespace MysteryDMS.FolderTree
{
    
    [PublishedAction(input_type: null, url = nameof(GetDMSFolderTree))]
    public class GetDMSFolderTree :
        BaseMysteryAction<ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            return new ContentActionOutput()
            {
                contents = new List<IContent>(
                    cd.GetAllByFilter<DMSFolder>(
                        x => x.parent_folders == null || x.parent_folders.Count == 0 ))
            };
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
