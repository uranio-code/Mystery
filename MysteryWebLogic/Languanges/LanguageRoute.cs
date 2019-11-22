using Mystery.Configuration;
using Mystery.Json;
using Mystery.Register;
using System.Web;
using System.Web.Routing;

namespace MysteryWebLogic.Languanges
{
    public class LanguagesRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new LanguagesHandler();
        }
    }

    public class LanguagesHandler : IHttpHandler
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
            var conf = this.getGlobalObject<IConfigurationProvider>();
            var dicts = conf.getConfiguration<LanguagesDictionaries>();
            dicts.ensure();
            HttpResponse response = context.Response;
            response.Write("var dicts = angular.fromJson(");
            var converter = this.getGlobalObject<IMysteryJsonConverter>();
            response.Write(converter.getJson(converter.getJson(dicts.data)));
            response.Write(");");
        }
    }
}
