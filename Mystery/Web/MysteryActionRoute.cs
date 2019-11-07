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
using System.IO.Compression;

namespace Mystery.Web
{

    public class PublishedAction : MysteryClassAttribute
    {

        public Type input_type { get; private set; }

        public Type output_type { get; private set; }

        /// <summary>
        /// instance published action
        /// </summary>
        /// <param name="input_type"></param>
        /// <param name="output_type">if left black ContentActionOutput will be assumed</param>
        public PublishedAction(Type input_type = null,Type output_type = null)
        {
            this.input_type = input_type;
            this.output_type = output_type;
            if(this.output_type ==null)
                this.output_type = typeof(ContentActionOutput);
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
                    typeof(MysteryActionRoute<,>)
                    .MakeGenericType(this.used_in,this.output_type));
            else
                instance = (IRouteHandler)Activator
                .CreateInstance(
                    typeof(MysteryActionRoute<,,>)
                    .MakeGenericType(this.used_in, input_type, this.output_type));

            RouteTable.Routes.Add(new Route(url, instance));
        }
    }



    class MysteryActionRoute<T, InputType, ActionResultType> : IRouteHandler where T : BaseMysteryAction<InputType, ActionResultType>, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MysteryActionRouteHandler<T, InputType, ActionResultType>();
        }
    }

    class MysteryActionRoute<T,ActionResultType> : IRouteHandler where T : BaseMysteryAction<ActionResultType>, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MysteryActionRouteHandler<T, ActionResultType>();
        }
    }

    public class MysteryActionRouteHandler<T, InputType, ActionResultType> : IHttpHandler where T : BaseMysteryAction<InputType, ActionResultType>, new()
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
                response.Filter = new GZipStream(response.Filter, CompressionLevel.Optimal);
                response.AppendHeader("Content-encoding", "gzip");
                response.Cache.VaryByHeaders["Accept-encoding"] = true;
                response.Write(json);
                response.Flush();
            }
        }
    }

    public class MysteryActionRouteHandler<T, ActionResultType> : IHttpHandler where T : BaseMysteryAction<ActionResultType>, new()
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
                response.Filter = new GZipStream(response.Filter, CompressionLevel.Optimal);
                response.AppendHeader("Content-encoding", "gzip");
                response.Cache.VaryByHeaders["Accept-encoding"] = true;
                response.Write(json);
                response.Flush();
            }
        }
    }
}




