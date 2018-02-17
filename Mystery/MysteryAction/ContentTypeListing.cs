using System;
using System.Collections.Generic;
using System.Linq;
using Mystery.Register;
using Mystery.Users;
using Mystery.Content;

namespace Mystery.MysteryAction
{
    public interface IContentTypeListing {
        bool canAccess(User user);
        IEnumerable<IContent> get(User user);
    }


    class ContentTypeListingAttribute : MysteryDefaultClassAttribute
    {
        public Type implementing_type { get; set; }

        private IContentTypeListing instance;

        public override void setUp()
        {
            if (implementing_type == null) implementing_type = typeof(AllCanList<>).MakeGenericType(used_in);
            if (!typeof(IContentTypeListing).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentTypeListing).FullName);
            }
            instance = (IContentTypeListing)this.getGlobalObject<FastActivator>().createInstance(implementing_type);

        }

        public IContentTypeListing getLister() {
            return instance;
        }

        private class AllCanList<T> : IContentTypeListing where T :IContent , new()
        {
            public bool canAccess(User user)
            {
                return true;
            }

            public IEnumerable<IContent> get(User user)
            {
                return 
                    from IContent x in this.getGlobalObject<IContentDispatcher>().GetAll<T>()
                    select x;
            }
        }
    }
}
