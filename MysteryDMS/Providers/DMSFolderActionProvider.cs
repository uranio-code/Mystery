using Mystery.UI;
using System.Collections.Generic;
using Mystery.Content;
using Mystery.Users;
using MysteryDMS.Actions;
using Mystery.Register;
using Mystery.MysteryAction;

namespace MysteryDMS.Model
{
    class DMSFolderActionProvider : IContentButtonProvider
    {

        static object _lock = new object();

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            lock (_lock) {
                var result = new List<IContentActionButton>();

                if (content == null || user == null)
                    return result;
                var type = content.GetType();
                if (!content.canAccess(user))
                    return result;
                if (user.isFavorite(content))
                    result.Add(new RemoveFromFavoriteAction(content, user));
                else
                    result.Add(new AddToFavoriteAction(content, user));
                var create_doc_action = new DMSCreateNewDocument(content, user);
                if (create_doc_action.Authorize())
                    result.Add(create_doc_action);
                var create_folder = new DMSCreateNewFolder(content, user);
                if (create_folder.Authorize())
                    result.Add(create_folder);
                var delete_action = new DMSDeleteFolder(content, user);
                if (delete_action.Authorize())
                    result.Add(delete_action);

                return result;
            }
            
        }
    }
}
