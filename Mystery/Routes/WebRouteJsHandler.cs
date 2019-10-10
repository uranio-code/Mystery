using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Mystery.Routes;

namespace Mystery.Routes
{

    public class WebRouteJs: IRouteHandler
    {

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new WebRouteJsHandler();
        }
    }

    public class WebRouteJsHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            
            string result = "var MysteryRoutes =  angular.fromJson(";
            
            using (WebActionExecutor executor = new WebActionExecutor()) {
                var routes = executor.executeAction(new GetRoutes());
                //we have now a string ready to be js but we what it to became an object
                //we need one more parse
                result += this.getGlobalObject<MysteryJsonConverter>().getJson(routes.json_output);
            };
            result += ");";
            response.Write(result);
            
        }
    }

    public class GetRoutes : BaseMysteryAction<IEnumerable<MysteryWebRoute>>, ICanRunWithOutLogin
    {
        protected override ActionResult<IEnumerable<MysteryWebRoute>> ActionImplemetation()
        {
            //the current instance db always get first priority
            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = new Dictionary<string, MysteryWebRoute>();
            IEnumerable<MysteryWebRoute> instace_routes = cd.GetAll<MysteryWebRoute>();
            foreach (var r in instace_routes) result[r.when] = r;

            //second is the code!
            foreach(Type provider_type in typeof(IMysteryWebRouteProvider).getChilds()){
                var instance = (IMysteryWebRouteProvider)this.getGlobalObject<FastActivator>().createInstance(provider_type);
                foreach (var r in instance.getRoutes())
                    if(!result.ContainsKey(r.when))
                        result[r.when] = r;
            }

            //if we still have no start is time to use the std one
            if (!result.ContainsKey("/start"))
                result["/start"] = new MysteryWebRoute
                {
                    when = "/start",
                    templateUrl = "MysteryWebContent/Shared/Start/Start.html",
                    controller = "StartController",
                };

            //time to see which type are accessible by UI and how
            //types view?
            IEnumerable < Type > content_types = this.getMystery().AssemblyRegister.getTypesMarkedWith<ContentTypeView>();
            foreach (var type in content_types) {
                ContentType ct = type.getMysteryAttribute<ContentType>();
                if (ct == null) continue; //? not a content type but still marked for view?
                string when = "/Type/" + ct.name;
                //always first priority to the instance
                if (result.ContainsKey(when)) continue;
                var ctv = type.getMysteryAttribute<ContentTypeView>();
                result[when] = new MysteryWebRoute
                {
                    when = when,
                    templateUrl = ctv.templateUrl,
                    controller = ctv.controller
                };
            }
            //instance view?
            content_types = this.getMystery().AssemblyRegister.getTypesMarkedWith<ContentView>();
            foreach (var type in content_types)
            {
                ContentType ct = type.getMysteryAttribute<ContentType>();
                if (ct == null) continue; //? not a content type but still marked for view?
                string when = "/" + ct.name + "/:tiny_guid";
                //always first priority to the instance
                if (result.ContainsKey(when)) continue;
                var ctv = type.getMysteryAttribute<ContentView>();
                result[when] = new MysteryWebRoute
                {
                    when = when,
                    templateUrl = ctv.templateUrl,
                    controller = ctv.controller,
                    Extra = nameof(ContentView),
                };
                //history view
                
                when = "/" + ct.name + "/:tiny_guid/history";
                //always first priority to the instance
                if (result.ContainsKey(when)) continue;
                result[when] = new MysteryWebRoute
                {
                    when = when,
                    templateUrl = "MysteryWebContent/History/ContentHistoryRoute.html",
                    controller = "ContentHistoryRouteController",
                };
            }

            return new ActionResult<IEnumerable<MysteryWebRoute>>(result.Values);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }

}
