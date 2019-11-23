using Mystery.Content;
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
using Users;

namespace Mystery.Users
{
    [AutoRegisteringService(url = nameof(UserService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    class UserService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult List()
        {
            
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new UserSuggestionsAction());
            }

        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult TOTPAuthenticationKey()
        {

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new TOTPAuthenticationKeyAction());
            }

        }

        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult TOTPAuthenticationCode(TOTPAuthenticationCodeInput input)
        {

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new TOTPAuthenticationCodeAction(),input.code);
            }

        }

        

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "ActivityStream/{guid}")]
        public WebActionResult ActivityStream(string guid)
        {
            Guid parsed = guid.fromTiny();
            if (parsed == Guid.Empty)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cd = this.getGlobalObject<IContentDispatcher>();
                return executor.executeAction(new UserActivityStream(),()=> cd.GetContent<User>(parsed));
            }
        }
    }
}
