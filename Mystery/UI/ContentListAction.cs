using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.Users;
using System;
using System.Collections.Generic;

namespace Mystery.UI
{

    public class ContentListAction : MysteryDefaultClassAttribute
    {
        public Type implementing_type { get; set; }

        private IContentButtonProvider _implemetation { get; set; }

        public override void setUp()
        {
            if (implementing_type == null) implementing_type = typeof(DefaultContentActionMenuProvider<>).MakeGenericType(used_in);
            if (!typeof(IContentButtonProvider).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentButtonProvider).FullName);
            }
            _implemetation = (IContentButtonProvider)this.getGlobalObject<FastActivator>().createInstance(implementing_type);

        }

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            return _implemetation.getActions(content, user);
        }

    }



}
