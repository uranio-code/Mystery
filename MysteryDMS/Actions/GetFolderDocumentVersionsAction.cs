using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using Mystery.Register;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Actions
{
    
    public class GetFolderDocumentVersionsAction : BaseMysteryAction<string, IEnumerable<IContent>>
    {
        protected override ActionResult<IEnumerable<IContent>> ActionImplemetation()
        {
            var cr = ContentReference.tryGetContentReferece(
                typeof(DMSFolder).getMysteryAttribute<ContentType>().name,
                input);
            if(cr==null)
                return ActionResultTemplates<IEnumerable<IContent>>.InvalidInput;
            var folder = (DMSFolder)cr.getContent();
            if (folder == null)
                return ActionResultTemplates<IEnumerable<IContent>>.InvalidInput;


            LinkedList<IContent> tmp = new LinkedList<IContent>();
            // DMS-91
            // we take the versions following the view permission
            foreach (DMSVersion version in folder.versions)
                if (version.view_permission_users.Contains(user) || user.account_type == Mystery.Users.UserType.admin)
                    tmp.AddFirst(version);

            ActionResult<IEnumerable<IContent>> result = new ActionResult<IEnumerable<IContent>>(tmp);
                       

            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            //to be confirmed
            //who has access to the folder can see document inside it
            var cr = ContentReference.tryGetContentReferece(
                typeof(DMSFolder).getMysteryAttribute<ContentType>().name,
                input);
            if (cr == null)
                return true;
            var folder = (DMSFolder)cr.getContent();
            if (folder == null)
                return true;
            var result =folder.canAccess(user);
            return result;
        }
    }
}
