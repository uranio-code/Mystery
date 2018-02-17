using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using Mystery.Web;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MysteryDMS.Actions
{
    [PublishedAction(input_type:typeof(ContentInput),url = nameof(DMSGetComments))]
    public class DMSGetComments : BaseMysteryAction<ContentInput, ContentActionOutput>
    {

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            if(!AuthorizeImplementation()) return ActionResultTemplates<ContentActionOutput>.UnAuthorized;

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var content = ContentReference.tryGetContentReferece(input.ContentType, input.tiny_guid)?.getContent();
            if (content == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var result = new ContentActionOutput();
            result.contents = new List<IContent>( 
                cd.GetAllByFilter<DMSComment>(x => x.parent_dms_content.guid == content.guid)
                .OrderBy(x => x.creation_date).Reverse()
                );
            return result;
        }

        /// <summary>
        /// https://calean.atlassian.net/browse/DMS-14
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var content = ContentReference.tryGetContentReferece(input.ContentType, input.tiny_guid)?.getContent();
            if (content == null)
                return false;
            //you must be able to see the content to see the comments
            var can_access = content.canAccess(user);
            if (!can_access)
                return false;

            if (content is DMSVersion) {
                DMSVersion version = (DMSVersion)content;
                if (version == null) return false;
                return version.comments_visible;
            }

            return true;
            
        }
    }
}
