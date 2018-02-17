using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Mystery.Web
{

    public class PublishedAction : MysteryClassAttribute
    {

        public Type input_type { get; private set; }

        public PublishedAction(Type input_type)
        {
            this.input_type = input_type;
        }

        public string url { get; set; }


        public override void setUp()
        {
            if (HttpContext.Current == null) return;

            url = string.IsNullOrEmpty(url) ? used_in.Name : url;
            IRouteHandler instance;
            if (input_type == null)
                instance = (IRouteHandler)Activator
                .CreateInstance(
                    typeof(MysteryActionRoute<>)
                    .MakeGenericType(this.used_in));
            else
                instance = (IRouteHandler)Activator
                .CreateInstance(
                    typeof(MysteryActionRoute<,>)
                    .MakeGenericType(this.used_in, input_type));

            RouteTable.Routes.Add(new Route(url, instance));
        }
    }



    class MysteryActionRoute<T, InputType> : IRouteHandler where T : BaseMysteryAction<InputType, ContentActionOutput>, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MysteryActionRouteHandler<T, InputType>();
        }
    }

    class MysteryActionRoute<T> : IRouteHandler where T : BaseMysteryAction<ContentActionOutput>, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MysteryActionRouteHandler<T>();
        }
    }

    public class MysteryActionRouteHandler<T, InputType> : IHttpHandler where T : BaseMysteryAction<InputType, ContentActionOutput>, new()
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
            //input json in the request we read it
            string input_json;
            using (var reader = new System.IO.StreamReader(request.InputStream))
            {
                input_json = reader.ReadToEnd();
            }
            
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var json_converter = this.getGlobalObject<MysteryJsonUiConverter>();

                var result = executor.executeAction(new T(), ()=> json_converter.readJson<InputType>(input_json));
                var json = json_converter.getJson(result);
                response.ContentType = "application/json; charset=utf-8";
                response.Write(json);
            }
        }
    }

    public class MysteryActionRouteHandler<T> : IHttpHandler where T : BaseMysteryAction<ContentActionOutput>, new()
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
            
            HttpResponse response = context.Response;

            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var json_converter = this.getGlobalObject<MysteryJsonConverter>();
                var result = executor.executeAction(new T());
                var json = json_converter.getJson(result);
                response.ContentType = "application/json; charset=utf-8";
                response.Write(json);
            }
        }
    }
}




