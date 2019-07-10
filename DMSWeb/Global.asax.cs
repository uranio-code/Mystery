using System;
using Mystery.Register;
using System.Web.Routing;
using System.IO;
using System.Reflection;
using Mystery.Web;
using Mystery.Applications;
using MysteryWebLogic;
using System.Collections.Generic;
using Mystery.Configuration;

namespace DMSWeb
{
    public class Global : System.Web.HttpApplication
    {
        private static MysteryWebBoot boot = new MysteryWebBoot();
        protected void Application_Start(object sender, EventArgs e)
        {
            MongoDbConfigurationProvider.default_env_name = "DMSWeb";
            boot.Assemblies.Add(typeof(MysteryWebLogic.Routes.GetRoutes).Assembly);
            boot.Assemblies.Add(typeof(MysteryDMS.Model.DMSVersion).Assembly);
            boot.Assemblies.Add(this.GetType().Assembly);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        private IEnumerable<string> apps = new HashSet<string>() { "DMS" };
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
            boot.ensureMystery(apps);
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