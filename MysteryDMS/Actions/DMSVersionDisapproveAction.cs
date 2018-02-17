using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Actions
{
    public class DMSVersionDisapproveAction : BaseContentAction
    {
        public DMSVersionDisapproveAction(IContent content, User user) : base(content, user)
        {
        }

        public override string label { get { return "DMS.VERSION.DISAPPROVE"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                // result.font_awesome_icon = "your owesome icon";
                // result.style = "your own style";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            // Set the wf
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);

            version.wf_Status = DMSConstants.wf_disapproved;
            version.next_actors.Clear();
            cd.Add(version);

            // use the signature API
            //throw new NotImplementedException();

            var result = new ContentActionOutput();
            result.changed_contents.Add(version);
            result.next_url = new ContentUrl(version);

            return result;
        }

        /// <summary>
        /// Under review is available to reviewers for under control documents.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;

            return version.approvers.Contains(this.user);

        }
    }
}