using System;
using Mystery.Register;
using System.Web.Routing;
using System.IO;
using System.Reflection;
using Mystery.Web;
using Mystery.Applications;
using Mystery.Routes;

namespace MysteryWebUi
{
    public class Global : System.Web.HttpApplication
    {
        private static bool _boot_done =  false;
        private static object _boot_lock = new object();
        
        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!_boot_done) {
                lock (_boot_lock) {
                    if (!_boot_done) DoBoot();
                }
            }
        }

        private void DoBoot()
        {
            this.getMystery().AssemblyRegister.Register(typeof(MysteryWebLogic.Authetication.SessionService).Assembly);
            this.getMystery().AssemblyRegister.Register(this.GetType().Assembly);

            var modules_folder = new DirectoryInfo( Server.MapPath("~/Modules"));
            if (modules_folder.Exists) {
                foreach (var f in modules_folder.GetFiles("*.dll")) {
                    this.getMystery().AssemblyRegister.Register(Assembly.LoadFile(f.FullName));
                }
            }

            using (var executor = new WebActionExecutor()) {
                executor.executeAction(new EnsureApplicationsAction());
            }

            RouteTable.Routes.Add(new Route("Dictionary", new MysteryWebLogic.Languanges.LanguagesRouteHandler()));
            RouteTable.Routes.Add(new Route("Routes", new WebRouteJs()));
            RouteTable.Routes.Add(new Route("ModulesJs", new ModuleJsRouteHandler()));
            

            RouteTable.Routes.Add(new Route("Directive/{name}", new UrlToDirective()));
            RouteTable.Routes.Add(new Route("Logged", new Mystery.Authentication.LiveIDLoginRoute()));
            if (RouteTable.Routes["singlePage"] == null)
                RouteTable.Routes.MapPageRoute("singlePage", "{*path}", "~/Default.aspx");



            _boot_done = true;
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            this.log().Error(Server.GetLastError());
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}