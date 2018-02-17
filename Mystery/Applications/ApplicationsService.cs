using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Applications
{
    [AutoRegisteringService(url = nameof(ApplicationsService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    public class ApplicationsService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult getApplications()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new GetApplicationsAction());
            }
        }
    }
}
