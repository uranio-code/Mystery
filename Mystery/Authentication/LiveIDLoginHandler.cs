using Mystery.Web;
using System.Web;
using System.Web.Routing;

namespace Mystery.Authentication
{
    public class LiveIDLoginRoute : IRouteHandler
    {

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new LiveIDLoginHandler();
        }
    }

    public class LiveIDLoginHandler : IHttpHandler
    {



        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        

        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string code = HttpContext.Current.Request["code"];

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                executor.executeAction(new DownloadLiveIDInfoAction(), code);
            }
            
            
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;


            response.Write(@"
<html>
    <body>
        <h1>just passing by</h1>
        <script>window.opener.loginCallBack(); window.close();</script>
    </body>
</html>");

        }


    }
}

    