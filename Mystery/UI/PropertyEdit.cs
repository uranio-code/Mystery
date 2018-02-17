using System;
using Mystery.Register;
using System.Collections.Generic;
using System.Linq;
using Mystery.Content;
using Mystery.Users;

namespace Mystery.UI
{

    public interface IPropertyEdit
    {

        bool canEdit(IContent content, User user);
    }

    public class AdminsCanEdit : IPropertyEdit
    {

        public bool canEdit(IContent content, User user)
        {
            return user != null && user.account_type == UserType.admin;
        }
    }
    public class ReadOnlyProperty : IPropertyEdit
    {

        public bool canEdit(IContent content, User user)
        {
            return false;
        }
    }

    public class PropertyEdit: MysteryPropertyAttribute
    {

        public Type implementing_type { get; set; }

        private IPropertyEdit instance;


        public override void setup()
        {

            if (implementing_type == null) implementing_type = typeof(AdminsCanEdit);
            if (!typeof(IPropertyEdit).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IPropertyEdit).FullName);
            }
            instance = (IPropertyEdit)Activator.CreateInstance(implementing_type);
        }

        public IPropertyEdit getPropertyEdit() {
            return instance;
        }

    }
}