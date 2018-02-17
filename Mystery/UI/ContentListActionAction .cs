using Mystery.Content;
using System;
using System.Collections.Generic;
using Mystery.Register;
using Mystery.MysteryAction;

namespace Mystery.UI
{
    public class ContentListActionAction : BaseMysteryAction<ContentReference, IEnumerable<IContentActionButton>>
    {

        protected override ActionResult<IEnumerable<IContentActionButton>> ActionImplemetation()
        {
            IContent content = input.getContent();
            if (content == null) return
                    ActionResultTemplates<IEnumerable<IContentActionButton>>.InvalidInput;

            return new ActionResult<IEnumerable<IContentActionButton>>(
                content.GetType().getMysteryAttribute<ContentListAction>().getActions(content,user)
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
