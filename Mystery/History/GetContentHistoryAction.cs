using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.History
{
    public class GetContentHistoryAction : BaseMysteryAction<ContentInput, IEnumerable<IPublishedAction>>
    {
        protected override ActionResult<IEnumerable<IPublishedAction>> ActionImplemetation()
        {
            var content = ContentReference.tryGetContentReferece(input.ContentType, input.tiny_guid)?.getContent();
            if (content == null)
                return ActionResultTemplates< IEnumerable<IPublishedAction>>.InvalidInput;

            var history = this.getGlobalObject<IHistoryRepository>();

            var content_history = history.GetByContent(content);

            return new ActionResult<IEnumerable<IPublishedAction>>(content_history);

        }

        protected override bool AuthorizeImplementation()
        {
            var content = ContentReference.tryGetContentReferece(input.ContentType, input.tiny_guid)?.getContent();
            if (content == null)
                return true;
            return content.canAccess(user);
        }
    }
}
