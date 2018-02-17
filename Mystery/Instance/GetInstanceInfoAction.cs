using Mystery.Files;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Instance
{
    /// <summary>
    /// return the current Mystery Instance View
    /// </summary>
    public class GetInstanceInfoAction : BaseMysteryAction<MysteryInstance>, ICanRunWithOutLogin
    {
        protected override ActionResult<MysteryInstance> ActionImplemetation()
        {
            return this.getGlobalObject<MysteryInstance>();
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
    /// <summary>
    /// return the current Mystery Instance View
    /// </summary>
    public class GetInstanceLogoAction : BaseMysteryAction<DownloadFileActionInput, MysteryFile>, ICanRunWithOutLogin
    {
        protected override ActionResult<MysteryFile> ActionImplemetation()
        {
            return this.executeAction(new DownloadFileAction(), input);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
