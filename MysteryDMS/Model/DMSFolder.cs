using System.Collections.Generic;
using Mystery.Content;
using Mystery.UI;
using Mystery.Users;
using Mystery.Register;

namespace MysteryDMS.Model
{
    [ContentType(label = "DMS.FOLDER.FOLDER", list_label = "DMS.FOLDER.FOLDERS")]
    [ContentView(templateUrl = "DMS/dms_folder_view.html")]
    [ContentTypeView]
    [ContentAction(implementing_type = typeof(DMSFolderActionProvider))]
    public class DMSFolder : BaseContent, IDMSContent

    {
        
        [ContentProperty(label = "DMS.FOLDER.DESCRIPTION")]
        [PropertyView]
        [PropertyEdit]
        public string description { get; set; } 


        [ContentProperty(label = "DMS.FOLDER.TITLE")]
        [ReferenceText()]
        [SearchText()]
        [PropertyView]
        [PropertyEdit]
        [PropertyColumn(template_url = "MysteryWebContent/MysteryContent/Properties/StringPropertyLinkedCell.html")]
        public string  title { get; set; } 

        [ContentProperty(label = "DMS.FOLDER.FOLDERS")]
        public MultiContentReference<DMSFolder> folders { get; set; } = new MultiContentReference<DMSFolder>();
        
        [ContentProperty(label = "DMS.FOLDER.VERSIONS")]
        [PropertyView]
        public MultiContentReference<DMSVersion> versions { get; set; } = new MultiContentReference<DMSVersion>();

        [ContentProperty(label = "DMS.FOLDER.VISIBLE")]
        [PropertyView]
        [PropertyEdit]
        public bool visible { get; set; } 
        

        [ContentProperty(label = "DMS.FOLDER.TAGS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSTag> tags { get; set; } = new MultiContentReference<DMSTag>();


        [ContentProperty(label = "DMS.FOLDER.UID")]
        public string  uid { get; set; } 

        [ContentProperty(label = "DMS.FOLDER.OWNER")]
        [PropertyView]
        [PropertyEdit]
        [PropertyColumn]
        [SingleReferencePropertyValuesProviderAtt]
        public ContentReference<User> rowner { get; set; } = new ContentReference<User>();

        [ContentProperty(label = "DMS.FOLDER.PARENT_FOLDER")]
        [PropertyView]
        public MultiContentReference<DMSFolder> parent_folders { get; set; } = new MultiContentReference<DMSFolder>();

        /// <summary>
        /// Folders allow you to set the permission both user level and user group level.
        /// </summary>
        [ContentProperty(label = "DMS.FOLDER.ADD_PERMISSION")]
        [PropertyView]
        [PropertyEdit]
        public MultiContentReference<User> add_permission_users { get; set; }

        [ContentProperty(label = "DMS.FOLDER.EDIT_PERMISSION")]
        [PropertyEdit]
        [PropertyView]
        public MultiContentReference<User> edit_permission_users { get; set; }

        [ContentProperty(label = "DMS.FOLDER.VIEW_PERMISSION")]
        [PropertyEdit]
        [PropertyView]
        public MultiContentReference<User> view_permission_users { get; set; }
        
        [ContentProperty(label = "DMS.FOLDER.ADD_PERMISSION_GROUP")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSUserGroup> add_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [ContentProperty(label = "DMS.FOLDER.EDIT_PERMISSION_GROUP")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSUserGroup> edit_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [ContentProperty(label = "DMS.FOLDER.VIEW_PERMISSION_GROUP")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSUserGroup> view_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [PropertyView]
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

        [PropertyView]
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

        [PropertyView]
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



        [ContentProperty(label = "DMS.FOLDER.SUBFOLDERS")]
        [PropertyView()]
        public MultiContentReference<DMSFolder> subfolders { get; set; } = new MultiContentReference<DMSFolder>();

        public IEnumerable<IDMSContent> children
        {
            get
            {
                List<IDMSContent> ret = new List<IDMSContent>();

                foreach(IDMSContent folder in subfolders.getAsContentEnum())
                {
                    ret.Add(folder);
                }

                foreach(IDMSContent version in versions.getAsContentEnum())
                {
                    ret.Add(version);
                }

                return ret;
            }
        }

       
        User IDMSContent.owner
        {
            get
            {
                return rowner;
            }

            set
            {
                rowner = value;
            }
        }

        public void add_subfolder(DMSFolder subfolder)
        {
            subfolder.parent_folders.Add(this);
            subfolders.Add(subfolder);

            // I store it to the database
            var cd = this.getGlobalObject<IContentDispatcher>();
            cd.Add(this);
            cd.Add(subfolder);
        }

    }
}
