using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Mystery.Register;
using Mystery.Configuration;
using Mystery.Web;
using Mystery.Authentication;
using System;
using Mystery.AccessHistory;

namespace MysteryWebLogic.Authetication
{
    [AutoRegisteringService(url = nameof(SessionService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    class SessionService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }
        

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string ID()
        {
            return this.getGlobalObject<MysterySession>().id;
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult me()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new GetMe());
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult LoggedEcho()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new LoggedEcho());
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult Logout()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new LogoutAction());
            }
        }

        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public string login(LoginInput input)
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new LoginAction(), input).isSuccessfull.ToString();
            }
        }

        [WebInvoke]
        public WebActionResult RegisterAccess(RegisterAccessInput input)
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new RegisterAccessAction(), input);
            }
        }

    }

   
}
