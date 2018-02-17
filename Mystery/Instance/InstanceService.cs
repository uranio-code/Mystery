using Mystery.Authentication;
using Mystery.Configuration;
using Mystery.Files;
using Mystery.Register;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mystery.Instance
{
    [AutoRegisteringService(url = nameof(InstanceService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    public class InstanceService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public LiveIDConfiguration getLiveIDConfiguration()
        {
            return this.getGlobalObject<IConfigurationProvider>().getConfiguration<LiveIDConfiguration>();
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult getInstance()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new GetInstanceInfoAction());
            }
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult getInstanceLogo()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(
                    new GetInstanceLogoAction(), 
                    () => {
                        var mystery_instance = this.getGlobalObject<MysteryInstance>();
                        return new DownloadFileActionInput()
                        {
                            content_reference = new Content.ContentReference(mystery_instance),
                            property_name = nameof(MysteryInstance.logo),
                            response = HttpContext.Current.Response,
                        };
                    } );
            }
        }
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult Search(SearchInput input)
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new SearchAction(), input);
            }
        }
    }
}
