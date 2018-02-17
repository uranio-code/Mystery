using Mystery.Content;
using Mystery.MysteryAction;
using System;
using System.Collections.Generic;
using System.Linq;
using Mystery.Register;

namespace Mystery.UI
{

    public class ContentTypeTableAction : BaseMysteryAction<Type,IEnumerable<ContentColumInfo> >,ICanRunWithOutLogin
    {
        protected override ActionResult<IEnumerable<ContentColumInfo>> ActionImplemetation()
        {
            if (input == null)
                return ActionResultTemplates<IEnumerable<ContentColumInfo>>.InvalidInput;
            var ct = input.getMysteryAttribute<ContentType>();
            if (ct == null)
                return ActionResultTemplates<IEnumerable<ContentColumInfo>>.InvalidInput;
            var ctt = input.getMysteryAttribute<ContentTypeTable>();
            if (ctt == null)
                return ActionResultTemplates<IEnumerable<ContentColumInfo>>.InvalidInput;
            return new ActionResult<IEnumerable<ContentColumInfo>>(ctt.getColumns());
        }

        protected override bool AuthorizeImplementation()
        {
            //all the checks are done in the doing, no need to give unauthorized
            return true;
        }
    }
}
