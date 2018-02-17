using Mystery.Content;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace MysteryWebLogic.Content
{
    [AutoRegisteringService(url = nameof(ContentEditService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    public class ContentEditService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "SingleReferenceSuggestions/{content_type_name}/{guid}/{property_name}?st={search_text}")]
        public WebActionResult SingleReferenceSuggestions(string content_type_name, string guid, string property_name, string search_text)
        {
            var cr = ContentReference.tryGetContentReferece(content_type_name,guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;
            if (string.IsNullOrEmpty(property_name))
                return WebActionResultTemplates.InvalidInput;
            var input = new PropertyEditSuggestionsActionInput() {
                content_reference = cr,
                property_name = property_name,
                search_text = search_text,
            };

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new SigleReferenceSuggestionsAction(), input);
            }
            
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "MultiReferenceSuggestions/{content_type_name}/{guid}/{property_name}?st={search_text}")]
        public WebActionResult MultiReferenceSuggestions(string content_type_name,string guid, string property_name, string search_text)
        {
            var cr = ContentReference.tryGetContentReferece(content_type_name, guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;
            if (string.IsNullOrEmpty(property_name))
                return WebActionResultTemplates.InvalidInput;
            var input = new PropertyEditSuggestionsActionInput()
            {
                content_reference = cr,
                property_name = property_name,
                search_text = search_text,
            };

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new MultiReferenceSuggestionsAction(), input);
            }

        }

    }
}
