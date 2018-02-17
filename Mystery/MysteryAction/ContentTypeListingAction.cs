using Mystery.Content;
using System;
using System.Collections.Generic;
using Mystery.Register;
using Mystery.UI;
using System.Linq;

namespace Mystery.MysteryAction
{
    public class ContentTypeListingAction : BaseMysteryAction<Type, IEnumerable<IContent>>
    {

        protected override ActionResult<IEnumerable<IContent>> ActionImplemetation()
        {
            var contents = input.getMysteryAttribute<ContentTypeListingAttribute>().getLister().get(user);
            return new ActionResult<IEnumerable<IContent>>(contents);
        }

        protected override bool AuthorizeImplementation()
        {
            return input.getMysteryAttribute<ContentTypeListingAttribute>().getLister().canAccess(user);
        }
    }
}
