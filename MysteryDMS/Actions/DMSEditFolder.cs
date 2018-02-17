using Mystery.Content;
using Mystery.Register;
using MysteryDMS.Model;
using System.Collections.Generic;
using Mystery.Users;
using Mystery.UI;
using Mystery.Security;
using System;

namespace MysteryDMS.Actions
{
    class DMSEditFolder : BaseContentAction
    {
        

        public DMSEditFolder(IContent content, User user) : base(content, user)
        {
        }

        public override string label
        {
            get
            {
                return "DMS.FOLDER.EDITFOLDER";
            }
        }

        /// <summary>
        /// This creates a new DMSVersion and redirects to its view page.
        /// </summary>
        /// <returns></returns>
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// S-DMS-REQ-35
        /// The S-DMS shall allow Users to create, edit and delete any content inside the “Training” folder.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSFolder folder = cd.GetContent<DMSFolder>(this.input.guid);
            if (folder == null) { return false; }
            if (folder.GetType() == typeof(DMSTrainingFolder)) { return true; }
            return folder.owner == this.user;
        }
    }
    }
