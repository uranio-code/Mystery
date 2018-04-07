using Mystery.Applications;
using Mystery.Register;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace MysteryWebLogic
{
    public class MysteryWebBoot
    {

        public MysteryWebBoot() {
            log4net.Config.XmlConfigurator.Configure();
        }

        public  bool boot_done { get {
                return _boot_done;
            } }

        private  bool _boot_done = false;
        private  object _boot_lock = new object();

        public  List<Assembly> Assemblies { get; set; } = new List<Assembly>();

        public  void ensureMystery(IEnumerable<string> application_names) {
            if (_boot_done)
                return;
            lock (_boot_lock)
            {
                if (!_boot_done) doBoot(application_names);
            }
            
        }

        private  void doBoot(IEnumerable<string> application_names) {
            
            foreach(var ass in Assemblies)
                this.getMystery().AssemblyRegister.Register(ass);

            using (var executor = new WebActionExecutor())
            {
                executor.executeAction(new EnsureApplicationsAction(), application_names);
            }

            RouteTable.Routes.Add(new Route("Dictionary", new MysteryWebLogic.Languanges.LanguagesRouteHandler()));
            RouteTable.Routes.Add(new Route("Routes", new MysteryWebLogic.Routes.WebRouteJs()));
            RouteTable.Routes.Add(new Route("ModulesJs", new MysteryWebLogic.Routes.ModuleJsRouteHandler()));


            RouteTable.Routes.Add(new Route("Directive/{name}", new MysteryWebLogic.Routes.UrlToDirective()));
            RouteTable.Routes.Add(new Route("Logged", new Mystery.Authentication.LiveIDLoginRoute()));
            if (RouteTable.Routes["singlePage"] == null)
                RouteTable.Routes.MapPageRoute("singlePage", "{*path}", "~/Default.aspx");
            _boot_done = true;
        }
    }
}
