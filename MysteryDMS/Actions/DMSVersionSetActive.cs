using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.UI;
using Mystery.Users;
using Mystery.Web;
using MysteryDMS.Model;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace MysteryDMS.Actions
{
    [AutoRegisteringService(url = nameof(DMSVersionService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    class DMSVersionService
    {

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        [WebInvoke (ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetActive/{guid}")]
        public WebActionResult SetActive(String guid)
        {
            

            using (WebActionExecutor executor = new WebActionExecutor())
            {

                DMSVersion version = this.getGlobalObject<IContentDispatcher>().GetContent<DMSVersion>(new Guid(guid));
                var action = new SetActiveAction();
                action.version = version;
                
                return executor.executeAction<WebActionResult>(action);

            }

            
        }

        
    }


    class SetActiveAction : BaseMysteryAction<WebActionResult>
    {
        
        public DMSVersion version;
        
        protected override ActionResult<WebActionResult> ActionImplemetation()
        {

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = this.version;

            WebActionResult result = new WebActionResult();

            version.active = true;
            cd.Add(version);

            // go back
            DMSVersion x = version.previous_version;
            while (x != null)
            {
                x.active = false;
                cd.Add(x);
                x = x.previous_version;
            }
            // go forward
            x = version.next_version;
            while (x != null)
            {
                x.active = false;
                cd.Add(x);
                x = x.next_version;
            }

            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            return version != null;
        }
    }
}
