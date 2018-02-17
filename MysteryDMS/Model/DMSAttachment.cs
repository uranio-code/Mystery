using Mystery.Content;
using System.Collections.Generic;
using Mystery.Users;
using Mystery.Files;

namespace MysteryDMS.Model
{
    [ContentType]
    public class DMSAttachment : BaseContent
    {
        
        public MultiContentReference<User> add_permission_users { get; set; }

        
        public MultiContentReference<User> edit_permission_users { get; set; }

        [ContentProperty(label = "DMS.VERSION.ORIGINAL_FILE")]
        public MysteryFile original_file { get; set; } 

        public User owner { get; set; } 
        
        public MultiContentReference<User> view_permission_users { get; set; }
    }
}
