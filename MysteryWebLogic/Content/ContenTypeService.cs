using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace MysteryWebLogic.Content
{
    [AutoRegisteringService(url = nameof(ContentTypeService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    public class ContentTypeService
    {

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        public class ContentTypeButtonData {
            public string template_url { get; set; }
            public string init_input_json { get; set; }
        }

        public class ContentTypeButtonsOuput
        {
            public List<ContentTypeButtonData> buttons { get; set; } = new List<ContentTypeButtonData>();
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public ContentTypeButtonsOuput ContentTypeButtons()
        {
            var result = new ContentTypeButtonsOuput();
            var converter = this.getGlobalObject<IMysteryJsonConverter>();
            foreach (var type in this.getMystery().AssemblyRegister.getTypesMarkedWith<ContentTypeButton>())
            {
                var ctb = type.getMysteryAttribute<ContentTypeButton>();
                result.buttons.Add(
                    new ContentTypeButtonData {
                        template_url = ctb.template_url,
                        init_input_json = converter.getJson(ctb.getInput())
                    });
            };
            return result;
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "ContentTypeTable/{content_type_name}")]
        public WebActionResult ContentTypeTable(string content_type_name)
        {
            var content_type = ContentType.getType(content_type_name);
            if (content_type == null)
                return WebActionResultTemplates.InvalidInput;
            if(content_type.getMysteryAttribute<ContentTypeTable>() == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new ContentTypeTableAction(), content_type);
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult Contents(string content_type_name)
        {
            if(ContentType.getType(content_type_name) == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new ContentTypeListingAction(), ContentType.getType(content_type_name));
            }

        }

    }
}
