using Mystery.Json;
using Mystery.Register;
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
    class MysteryServiceRoute<T> : IRouteHandler where T : new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MysteryServiceHandler<T>();
        }
    }

    class MysteryServiceHandler<T> : IHttpHandler  where T :new() 
    {

        private T _instace = new T();
        private Dictionary<string, MethodInfo> map = new Dictionary<string, MethodInfo>();
        public MysteryServiceHandler() {
            foreach (var m in typeof(T).GetMethods()) {
                map[m.Name.ToLower()] = m;
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        private static object[] RetriveMethodParamenters(MethodInfo method, HttpRequest request)
        {
            var result = new List<object>();
            var left = new List<int>();
            ParameterInfo[] parameters = method.GetParameters();
            var json_converter = result.getGlobalObject<MysteryJsonConverter>();
            for (var i = 0; i < parameters.Length; i++) {
                var pi = parameters[i];
                if (request[pi.Name] != null)
                {
                    //we have the parameter in the query string
                    //we try to parse it
                    var converter = TypeDescriptor.GetConverter(pi.ParameterType);
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        result.Add(converter.ConvertFrom(request[pi.Name]));
                    }
                    else
                    {
                        //we allow json in the request
                        result.Add(json_converter.readJson(request[pi.Name], pi.ParameterType));
                    }
                }
                else
                {
                    //this parameter could be in the request stream
                    //or it is left empty in this request
                    result.Add(pi);
                    left.Add(i);
                }
            }

            //we have read all the parameter we could for the query string
            //and only 1 is allowed to be in the request stream
            //let's see if we have something in the request stream

            string request_content;
            using (var reader = new System.IO.StreamReader(request.InputStream)) {
                request_content = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(request_content))
            {
                //we found nothing in the stream, all missing parameters are null
                foreach (var i in left) result[i] = null;
            }
            //we found something, to follow a little the std, we use for the last left over
            else if (left.Count > 0)
            {
                ParameterInfo pi = (ParameterInfo)result[left.LastOrDefault()];
                //all the rest will be null
                foreach (var i in left) result[i] = null;
                var converter = TypeDescriptor.GetConverter(pi.ParameterType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    result[left.LastOrDefault()] = converter.ConvertFrom(request_content);
                }
                else
                {
                    //we allow json in the request
                    result[left.LastOrDefault()] = json_converter.readJson(request_content, pi.ParameterType);
                }
            }
            else {
                //bizarre case, we have something but all parameter are already filled
            }

            
            return result.ToArray();
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            var method_name = (string)request.RequestContext.RouteData.Values["name"];
            method_name = method_name.ToLower().Trim();
            var method_info = map[method_name];
            object result = method_info.Invoke(
                _instace, 
                RetriveMethodParamenters(method_info,request));
            if (!method_info.ReturnType.Equals(typeof(void))) {
                var converter = this.getGlobalObject<MysteryJsonConverter>();
                var json = converter.getJson(result);
                response.ContentType = "application/json; charset=utf-8";
                response.Write(json);
            }
        }
    }

}
