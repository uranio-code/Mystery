﻿using Mystery.Content;
using Mystery.MysteryAction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.UI
{

    public class ContentGetActionInfoActionInput 
    {
        public ContentReference content_reference { get; set; }
       
        public string action_name { get; set; }
    }


    public class ContentGetActionInfoAction : BaseMysteryAction<ContentGetActionInfoActionInput, IContentActionButton>
    {
        protected override ActionResult<IContentActionButton> ActionImplemetation()
        {
            IEnumerable<IContentActionButton> menu = this.executeAction(new ContentActionMenuAction(), input.content_reference);
            IContentActionButton action = (from x in menu where x.name == input.action_name select x).FirstOrDefault();
            if (action == null)
                return new ActionResult<IContentActionButton> { isSuccessfull = false, message = "action not found" };
            else
                return new ActionResult<IContentActionButton>(action);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
