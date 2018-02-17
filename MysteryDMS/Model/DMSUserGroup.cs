using Mystery.Content;
using Mystery.Register;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model.Providers;
using System.Collections.Generic;

namespace MysteryDMS.Model
{
    [ContentType(label = "DMS.USERGROUP", list_label = "DMS.USERGROUPS")]
    [ContentTypeButton(template_url = "MysteryWebContent/MysteryContent/TypeButton.html")]
    [ContentTypeView(templateUrl = "DMS/DMSUserGroups.html")]
    [ContentView(templateUrl = "DMS/DMSUserGroupView.html")]
    [ContentAction(implementing_type = typeof(DMSUserGroupActionProvider))]
    [ContentTypeTable]
    public class DMSUserGroup : BaseContent
    {
        [ContentProperty(label = "DMS.USERGROUP.TITLE")]
        [PropertyView]
        [PropertyEdit]
        [ReferenceText]
        [PropertyColumn(template_url = "MysteryWebContent/MysteryContent/Properties/StringPropertyLinkedCell.html")]
        public string title { get; set; }

        [ContentProperty(label = "DMS.USERGROUP.SUBGROUPS")]
        [PropertyView]
        public MultiContentReference<DMSUserGroup> sub_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        /// <summary>
        /// This property contains all the users stored directly in this group
        /// </summary>
        [ContentProperty(label = "DMS.USERGROUP.MEMBERS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> members { get; set; } = new MultiContentReference<User>();

        private MultiContentReference<User> _all_members = null;

        /// <summary>
        /// This property gets the users from all the subgroups
        /// </summary>
        [ContentProperty(label = "DMS.USERGROUP.ALL_MEMBERS")]
        [PropertyView]
        public MultiContentReference<User> all_members {
            get
            {

                _all_members = new MultiContentReference<User>();
                _all_members.AddRange(members);

                explore_subgroups(this, _all_members);

                return _all_members;
            }
        }

        /// <summary>
        /// This recourse routine exports 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="result"></param>
        private void explore_subgroups(DMSUserGroup root, MultiContentReference<User> result)
        {
            foreach (DMSUserGroup group in root.sub_groups)
            {
                explore_subgroups(group, result);
                // I add only the members directly stored in the group,
                // if here I add theall_members then the recoursion explodes exponentially.
                result.AddRange(group.members);
            }
        }

        /// <summary>
        /// This return the parent group for this particular group.
        /// </summary>
        [ContentProperty(label = "DMS.USERGROUP.PARENT_GROUP")]
        [PropertyView]
        public ContentReference<DMSUserGroup> parent_group { get; set; } = new ContentReference<DMSUserGroup>();

        public ContentReference<User> creator { get; set; } = new ContentReference<User>();

        public void deleteSubgroups()
        {
            if (sub_groups == null || sub_groups.Count == 0) return;

            foreach (DMSUserGroup group in sub_groups)
                group.deleteSubgroups();

            var cd = this.getGlobalObject<IContentDispatcher>();
            cd.Remove(this);
        }


        // permissions usage
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
                if (edit_permission_users!=null) _edit_permission.AddRange(edit_permission_users);
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
                if (add_permission_users!=null) _add_permission.AddRange(add_permission_users);
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
                if (view_permission_users!=null) _view_permission.AddRange(view_permission_users);
                return _view_permission;
            }
        }


        // permissions for users
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

        // permissions for groups
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
    }
}
