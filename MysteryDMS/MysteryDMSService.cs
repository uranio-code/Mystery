using Mystery.Content;
using Mystery.Web;
using MysteryDMS.Actions;
using MysteryDMS.StartPage;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;


namespace MysteryDMS
{
    [AutoRegisteringService(url = nameof(MysteryDMSService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    class MysteryDMSService
    {

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult GetLastDMSActions()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new GetLastDMSActions());
            }
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult GetFolderDocumentVersions(string folder)
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new GetFolderDocumentVersionsAction(), folder);
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "paths/{content_type_name}/{tiny_guid}")]
        public WebActionResult GetPaths(string content_type_name,string tiny_guid)
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cr = ContentReference.tryGetContentReferece(content_type_name, tiny_guid);
                if (cr == null) {
                    return WebActionResultTemplates.InvalidInput;
                }
                return executor.executeAction(new GetDmsPathsAction(), cr);
            }
        }
    }
}
