using Mystery.Content;
using Mystery.Users;
using System.Collections.Generic;

namespace MysteryDMS.Model
{
    public interface IDMSContent : IContent
    {
        User owner { get; set; }

        IEnumerable<IDMSContent> children { get; }

        MultiContentReference<User> add_permission_users { get; set; }

        MultiContentReference<User> edit_permission_users { get; set; }

        MultiContentReference<User> view_permission_users { get; set; }

        MultiContentReference<DMSUserGroup> add_permission_groups { get; set; }

        MultiContentReference<DMSUserGroup> edit_permission_groups { get; set; }

        MultiContentReference<DMSUserGroup> view_permission_groups { get; set; }
    }
}
