using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using System;

namespace Mystery.UI
{
    public class ContentViewAction : BaseMysteryAction<ContentReference, IContent>
    {
        protected override ActionResult<IContent> ActionImplemetation()
        {
            return new ActionResult<IContent>(input.getContent());
        }

        protected override bool AuthorizeImplementation()
        {
            IContent content = input.getContent();
            if (content == null) return true;
            return content.canAccess(user);
        }
    }
}
