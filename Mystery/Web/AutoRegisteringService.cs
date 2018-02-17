using Mystery.Register;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace Mystery.Web
{
    public class AutoRegisteringService : MysteryClassAttribute
    {

        public bool useMysteryHandlers { get; set; }

        public string url { get; set; }


        public override void setUp()
        {
            if (HttpContext.Current == null) return;
            url = string.IsNullOrEmpty(url) ? used_in.FullName.Replace(".", "/") : url;
            if (!this.useMysteryHandlers)
                RouteTable.Routes.Add(new ServiceRoute(url, new AutoRegisteredWebServiceHostFactory(used_in), used_in));
            else
            {
                var instace = (IRouteHandler)this.getGlobalObject<FastActivator>()
                    .createInstance(
                    typeof(MysteryServiceRoute<>)
                    .MakeGenericType(this.used_in));

                RouteTable.Routes.Add(new Route(url + "/{name}",  instace));
            }
        }
    }

}
