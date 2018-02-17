using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;
using Mystery.Files;

namespace MysteryDMS.Actions
{
    class DMSVersionDownload : BaseContentAction
    {
        public DMSVersionDownload(IContent content, User user) : base(content, user)
        {
        }
        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "download";
                result.style = "success btn-outline";
                return result;
            }
        }
        public override string label
        {
            get
            {
                return "DMS.VERSION.DOWNLOAD";
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            if(!AuthorizeImplementation()) return ActionResultTemplates<ContentActionOutput>.UnAuthorized;

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            if(!(input is DMSVersion))
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;
            DMSVersion version = (DMSVersion)input;
            
            return new ContentActionOutput {next_url = new FilePropertyUrl(version,nameof(DMSVersion.original_file) )};
        }

        /// <summary>
        /// https://calean.atlassian.net/browse/DMS-60
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = (DMSVersion)input;
            if (version == null) return false;
            return true;
        }
    }
}
