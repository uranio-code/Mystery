using System;
using Mystery.Register;
using Mystery.Users;
using Mystery.Content;

namespace Mystery.MysteryAction
{
    public interface IContentDelete
    {
        bool canDelete(IContent content, User user);
    }


    public class ContentDeleteAttribute : MysteryDefaultClassAttribute
    {
        public Type implementing_type { get; set; }

        private IContentDelete instance;

        public override void setUp()
        {
            if (implementing_type == null) implementing_type = typeof(AllCanDelete);
            if (!typeof(IContentDelete).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentDelete).FullName);
            }
            instance = (IContentDelete)this.getGlobalObject<FastActivator>().createInstance(implementing_type);

        }

        public bool canDelete(IContent content, User user) {
            return instance.canDelete(content, user);
        }

        private class AllCanDelete : IContentDelete
        {
            bool IContentDelete.canDelete(IContent content,User user)
            {
                return true;
            }
        }
    }
}
