using Mystery.UI;
using System.Collections.Generic;
using Mystery.Content;
using Mystery.Users;
using MysteryDMS.Actions;

namespace MysteryDMS.Model
{
    class DMSVersionActionListProvider : IContentButtonProvider
    {
        private IContentButtonProvider _default = new DefaultContentActionMenuProvider<DMSVersion>();

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            
            var result = new List<IContentActionButton>();
            if (!(content is DMSVersion))
                return new List<IContentActionButton>();
            var version = (DMSVersion)content;
            if (version.original_file != null)
                result.Add(new DMSVersionDownload(content,user));

            if (user.isFavorite(content))
                result.Add(new RemoveFromFavoriteAction(content, user));
            else
                result.Add(new AddToFavoriteAction(content, user));

            result.Add(new DMSDeleteVersion(version, user));
            
            return result;
        }
    }
}
