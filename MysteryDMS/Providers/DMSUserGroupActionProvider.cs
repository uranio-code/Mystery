
using System.Collections.Generic;
using Mystery.Content;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Actions;

namespace MysteryDMS.Model.Providers
{
    class DMSUserGroupActionProvider : IContentButtonProvider
    {

        private IContentButtonProvider _default = new DefaultContentActionMenuProvider<DMSVersion>();

        IEnumerable<IContentActionButton> IContentButtonProvider.getActions(IContent content, User user)
        {
            var result = new List<IContentActionButton>();
            result.AddRange(_default.getActions(content, user));
            result.Add(new DMSCreateSubgroup(content, user));
            result.Add(new DMSDeleteSubgroup(content, user));
            return result;
        }
    }
}
