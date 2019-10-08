using System.Web;
using System.Web.Routing;

namespace Mystery.Routes
{
    public class UrlToDirective : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new UrlToDirectiveJsHandler();
        }
    }

    class UrlToDirectiveJsHandler : IHttpHandler
    {
        public bool IsReusable{get{return true;}}

        
        private string snake_case(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            string result = name.Substring(0,1).ToLower();
            name = name.Substring(1);
            if (string.IsNullOrEmpty(name)) return result;
            foreach (char c in name) {
                if (char.IsUpper(c))
                    result += "-" + char.ToLower(c);
                else
                    result += c;
                
            }
            return result;
        }


        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            var directive = snake_case((string)request.RequestContext.RouteData.Values["name"]);

            string result = "<" + directive + "></" + directive +">";
            
            response.Write(result);
        }
    }
}
