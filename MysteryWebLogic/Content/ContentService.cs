using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Mystery.History;

namespace MysteryWebLogic.Content
{
    [AutoRegisteringService(url = nameof(ContentService))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceContract(Namespace = "")]
    public class ContentService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public string Hello()
        {
            return "hello";
        }

        
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "ContentView/{content_type_name}/{guid}")]
        public WebActionResult ContentView(string content_type_name,string guid)
        {
            var cr = ContentReference.tryGetContentReferece(content_type_name, guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new ContentViewAction(), cr);
            }
            
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "OldContentView/{guid}")]
        public WebActionResult OldContentView(string guid)
        {
            var cr = ContentReference.tryGetContentReferece(nameof(BaseContent), guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new ContentViewAction(), cr);
            }

        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{content_type_name}/{guid}/actions")]
        public WebActionResult ActionMenu(string content_type_name,string guid)
        {
            var input = ContentReference.tryGetContentReferece(content_type_name, guid);
            if (input == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                WebActionResult result = executor.executeAction(new ContentActionMenuAction(), input);
                return result;
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{content_type_name}/{guid}/workflow_actions")]
        public WebActionResult WorkflowActionMenu(string content_type_name, string guid)
        {
            var input = ContentReference.tryGetContentReferece(content_type_name, guid);
            if (input == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                WebActionResult result = executor.executeAction(new ContentWorkflowActionMenuAction(), input);
                return result;
            }
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{content_type_name}/{guid}/list-actions")]
        public WebActionResult ListActions(string content_type_name,string guid)
        {
            var input = ContentReference.tryGetContentReferece(content_type_name, guid);
            if (input == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                WebActionResult result = executor.executeAction(new ContentListActionAction(), input);
                return result;
            }
        }
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{content_type_name}/{guid}/action-info/{action_name}")]
        public WebActionResult getActionInfo(string content_type_name, string guid,string action_name)
        {
            if (string.IsNullOrEmpty(action_name))
                return WebActionResultTemplates.InvalidInput;
            
            var cr = ContentReference.tryGetContentReferece(content_type_name,guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;

            var input = new ContentGetActionInfoActionInput();
            input.content_reference = cr;
            input.action_name = action_name;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                WebActionResult result = executor.executeAction(new ContentGetActionInfoAction(), input);
                return result;
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{content_type_name}/{guid}/workflow-action-info/{action_name}")]
        public WebActionResult getWorkflowActionInfo(string content_type_name, string guid, string action_name)
        {
            if (string.IsNullOrEmpty(action_name))
                return WebActionResultTemplates.InvalidInput;

            var cr = ContentReference.tryGetContentReferece(content_type_name, guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;

            var input = new ContentGetWorkflowActionInfoActionInput();
            input.content_reference = cr;
            input.action_name = action_name;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                WebActionResult result = executor.executeAction(new ContentGetWorkflowActionInfoAction(), input);
                return result;
            }
        }

                
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult ExecuteWorkflowAction(ExecuteContentActionInput input)
        {
            if (string.IsNullOrEmpty(input.action_name) ||
                string.IsNullOrEmpty(input.content_type_name) ||
                string.IsNullOrEmpty(input.guid))
                return WebActionResultTemplates.InvalidInput;


            var cr = ContentReference.tryGetContentReferece(input.content_type_name, input.guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var content = cr.getContent();
                if (content == null)
                    return WebActionResultTemplates.InvalidInput;

                MysterySession session = this.getGlobalObject<MysterySession>();

                IEnumerable<IContentActionButton> menu = content.GetType()
                    .getMysteryAttribute<ContentWorkflowAction>()
                    .getActions(content, session.user);

                IContentActionButton action = (from x in menu
                                               where x.name == input.action_name
                                               select x).FirstOrDefault();

               
                if (action == null)
                    return WebActionResultTemplates.InvalidInput; 

                //ones we reach here indeed the action is found, but it might not be good.
                //an IContentActionButton might not be a BaseContentAction, when for example we need more input
                //BaseContentAction all they require  the content
                if (!(action is BaseContentAction))
                    return WebActionResultTemplates.InvalidInput;

                var content_action = (BaseContentAction)action;

                WebActionResult result = executor.executeAction(content_action, content);

                return result;
            }
        }

        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult ExecuteAction(ExecuteContentActionInput input)
        {
            if (string.IsNullOrEmpty(input.action_name)||
                string.IsNullOrEmpty(input.content_type_name)||
                string.IsNullOrEmpty(input.guid))
                return WebActionResultTemplates.InvalidInput;

            
            var cr = ContentReference.tryGetContentReferece(input.content_type_name,input.guid);
            if (cr == null)
                return WebActionResultTemplates.InvalidInput;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var content = cr.getContent();
                if(content==null)
                    return WebActionResultTemplates.InvalidInput;

                MysterySession session = this.getGlobalObject<MysterySession>();

                IEnumerable<IContentActionButton> menu = content.GetType()
                    .getMysteryAttribute<ContentAction>()
                    .getActions(content, session.user);

                IContentActionButton action = (from x in menu
                                               where x.name == input.action_name
                                               select x).FirstOrDefault();

                if (action == null) {
                    //it might be from the list instead
                    menu = content.GetType()
                        .getMysteryAttribute<ContentListAction>()
                        .getActions(content, session.user);
                    action = (from x in menu
                              where x.name == input.action_name
                              select x).FirstOrDefault();
                }

                if (action == null)
                {
                    //it might be a workflow instead
                    menu = content.GetType()
                        .getMysteryAttribute<ContentWorkflowAction>()
                        .getActions(content, session.user);
                    action = (from x in menu
                              where x.name == input.action_name
                              select x).FirstOrDefault();
                }

                if (action == null)
                    return WebActionResultTemplates.InvalidInput;

                //ones we reach here indeed the action is found, but it might not be good.
                //an IContentActionButton might not be a BaseContentAction, when for example we need more input
                //BaseContentAction all they require  the content
                if(!(action is BaseContentAction))
                    return WebActionResultTemplates.InvalidInput;

                var content_action = (BaseContentAction)action;

                WebActionResult result = executor.executeAction(content_action,content);

                return result;
            }
        }

        public class ExecuteContentActionInput
        {
            public string content_type_name { get; set; }
            public string guid { get; set; }
            public string action_name { get; set; }

        }

        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public WebActionResult History(ContentInput input)
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                return executor.executeAction(new GetContentHistoryAction(),input);
            }
        }

    }
}
