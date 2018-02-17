using Mystery.UI;
using System.Collections.Generic;
using Mystery.Content;
using Mystery.Users;
using MysteryDMS.Actions;

namespace MysteryDMS.Model
{
    class DMSVersionActionProvider : IContentButtonProvider
    {
        private IContentButtonProvider _default = new DefaultContentActionMenuProvider<DMSVersion>();

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            
            var result = new List<IContentActionButton>();
            if (!(content is DMSVersion))
                return new List<IContentActionButton>();
            var version = (DMSVersion)content;
            result.AddRange(_default.getActions(version, user));
            result.RemoveAt(0);
            result.Add(new DMSAddVersion(version, user));
            result.Add(new DMSDeleteVersion(version, user));
            //result.Add(new DMSDeleteDocument(version, user));
            //if (version.allow_comments) result.Add(new DMSAddCommentAction(version, user));
            

           return result;
        }
    }
}
