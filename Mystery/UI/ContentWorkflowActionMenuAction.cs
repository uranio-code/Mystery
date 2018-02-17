using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using System.Collections.Generic;

namespace Mystery.UI
{
    public class ContentWorkflowActionMenuAction : BaseMysteryAction<ContentReference, IEnumerable<IContentActionButton>>
    {

        protected override ActionResult<IEnumerable<IContentActionButton>> ActionImplemetation()
        {
            IContent content = input.getContent();
            if (content == null) return new ActionResult<IEnumerable<IContentActionButton>>() { isSuccessfull = false, UnAuthorized = false, message = "content not found" };
            return new ActionResult<IEnumerable<IContentActionButton>>(
                content.GetType().getMysteryAttribute<ContentWorkflowAction>().getActions(content, user)
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
