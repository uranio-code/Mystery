using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using Mystery.Register;
using System.Web;
using Mystery.Content;

namespace Mystery.Files
{
    [AutoRegisteringService(url = nameof(FileService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    public class FileService
    {
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult PostNewFile()
        {
            var input = new List<AddFileInput>();
            foreach (string filename in HttpContext.Current.Request.Files) {
                HttpPostedFile file = HttpContext.Current.Request.Files[filename];
                input.Add(new AddFileInput() { filename = file.FileName, stream = file.InputStream });
            }
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new AddFilesAction(), input);
            }

        }


        [WebGet(UriTemplate = "{content_type_name}/{guid}/{property_name}")]
        public WebActionResult getFile(string content_type_name,string guid, string property_name)
        {

            if (string.IsNullOrEmpty(property_name))
                return WebActionResultTemplates.InvalidInput;

            var input = new DownloadFileActionInput()
            {
                content_reference = ContentReference.tryGetContentReferece(content_type_name,guid),
                property_name = property_name,
                response = HttpContext.Current.Response,
            };

            if (input.content_reference == null)
                return WebActionResultTemplates.InvalidInput;


            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new DownloadFileAction(), input);
            }

        }
    }
}
