using Mystery.Content;
using System;
using System.Collections.Generic;
using Mystery.Register;
using Mystery.MysteryAction;

namespace Mystery.UI
{
    public class ContentActionMenuAction : BaseMysteryAction<ContentReference, IEnumerable<IContentActionButton>>
    {

        protected override ActionResult<IEnumerable<IContentActionButton>> ActionImplemetation()
        {
            IContent content = input.getContent();
            if (content == null) return new ActionResult<IEnumerable<IContentActionButton>>() { isSuccessfull = false, UnAuthorized = false, message = "content not found" };
            return new ActionResult<IEnumerable<IContentActionButton>>(
                content.GetType().getMysteryAttribute<ContentAction>().getActions(content,user)
                );
        }

        protected override bool AuthorizeImplementation()
        {
            IContent content = input.getContent();
            if (content == null) return true;   
            return content.canAccess(user);
        }
    }
}
