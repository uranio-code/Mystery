using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Applications
{
    public class GetApplicationsAction : BaseMysteryAction<IEnumerable<MysteryApplication>>, ICanRunWithOutLogin
    {
        protected override ActionResult<IEnumerable<MysteryApplication>> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = cd.GetAll<MysteryApplication>();
            return new ActionResult<IEnumerable<MysteryApplication>>(result);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
