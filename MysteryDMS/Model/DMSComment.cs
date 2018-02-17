using Mystery.Content;
using System.Collections.Generic;
using Mystery.Users;
using System;
using Mystery.UI;

namespace MysteryDMS.Model
{
    [ContentType]
    public class DMSComment : BaseContent
    {

        [ContentProperty]
        public MultiContentReference<User> add_permission_users { get; set; }

        [ContentProperty]
        public MultiContentReference<User> edit_permission_users { get; set; }


        [ContentProperty]
        [PropertyView]
        public DateTime creation_date { get; set; } = DateTime.Now;

        [PropertyView]
        public String creation_date_view
        {
            get
            {
                return creation_date.ToLongTimeString();
            }
        } 

        [ContentProperty, PropertyView]
        public String panel_title
        {
            get
            {
                if(creation_date == DateTime.MinValue)
                {
                    return "";
                }

                if(owner.value == null)
                {
                    return "";
                }

                return owner.value.fullname + " - " + creation_date.ToShortDateString();
            }
        }

        [ContentProperty]
        [PropertyView]
        public ContentReference<User> owner { get; set; }

        [ContentProperty]
        [PropertyView]
        public ContentReference<DMSVersion> parent_dms_content { get; set; }
        

        [ContentProperty(label = "DMS.VERSION.COMMENT_BODY")]
        [PropertyView]
        public string  comment_body { get; set; }

        [ContentProperty]
        public MultiContentReference<User> view_permission_users { get; set; }

        [ContentProperty(label = "DMS.FOLDER.ADD_PERMISSION_GROUP")]
        public MultiContentReference<DMSUserGroup> add_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [ContentProperty(label = "DMS.FOLDER.EDIT_PERMISSION_GROUP")]
        public MultiContentReference<DMSUserGroup> edit_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [ContentProperty(label = "DMS.FOLDER.VIEW_PERMISSION_GROUP")]
        public MultiContentReference<DMSUserGroup> view_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        private MultiContentReference<User> _edit_permission = new MultiContentReference<User>();
        public MultiContentReference<User> edit_permission
        {
            get
            {
                foreach (DMSUserGroup group in edit_permission_groups.getAsContentEnum())
                {
                    _edit_permission.AddRange(group.all_members);
                }
                _edit_permission.AddRange(edit_permission_users);
                return _edit_permission;
            }
        }


        private MultiContentReference<User> _add_permission = new MultiContentReference<User>();
        public MultiContentReference<User> add_permission
        {
            get
            {
                foreach (DMSUserGroup group in add_permission_groups.getAsContentEnum())
                {
                    _add_permission.AddRange(group.all_members);
                }
                _add_permission.AddRange(add_permission_users);
                return _add_permission;
            }
        }


        private MultiContentReference<User> _view_permission = new MultiContentReference<User>();
        public MultiContentReference<User> view_permission
        {
            get
            {
                foreach (DMSUserGroup group in view_permission_groups.getAsContentEnum())
                {
                    _view_permission.AddRange(group.all_members);
                }
                _view_permission.AddRange(view_permission_users);
                return _view_permission;
            }
        }
    }
}
