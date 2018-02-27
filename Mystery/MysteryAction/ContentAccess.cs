using System;
using Mystery.Register;
using Mystery.Users;
using Mystery.Content;

namespace Mystery.MysteryAction
{
    public interface IContentAccess {
        bool canAccess(IContent content, User user);
    }
    public class AllCanAccess : IContentAccess
    {
        bool IContentAccess.canAccess(IContent content, User user)
        {
            return true;
        }
    }
    public class OnlyAdminCanAccess : IContentAccess
    {
        bool IContentAccess.canAccess(IContent content, User user)
        {
            return user != null && user.account_type == UserType.admin;
        }
    }

    public class ContentAccessAttribute : MysteryDefaultClassAttribute
    {
        public Type implementing_type { get; set; }

        private IContentAccess instance;

        public override void setUp()
        {
            if (implementing_type == null) implementing_type = typeof(AllCanAccess);
            if (!typeof(IContentAccess).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentAccess).FullName);
            }
            instance = (IContentAccess)this.getGlobalObject<FastActivator>().createInstance(implementing_type);

        }

        public bool canAccess(IContent content, User user) {
            return instance.canAccess(content, user);
        }

    }
}
