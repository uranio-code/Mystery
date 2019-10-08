using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace Mystery.Routes
{
    public class ModuleJsRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ModuleJsHandler();
        }
    }

    class ModuleJsHandler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        IEnumerable<ConfigurationElement> GetVirtualDirectories(string siteName,string applicationPath)
        {

            ConfigurationSection sitesSection =WebConfigurationManager.GetSection("system.applicationHost/sites");


            ConfigurationElement siteElement =
                Find(sitesSection.GetCollection(), "name", siteName);

            if (siteElement == null)
            {
                throw new InvalidOperationException("Invalid Site");
            }

            ConfigurationElement applicationElement =
                Find(siteElement.GetCollection(), "path", applicationPath);

            if (applicationElement == null)
            {
                throw new InvalidOperationException("Invalid Application");
            }

            return applicationElement.GetCollection();
        }

        private static ConfigurationElement Find(
            ConfigurationElementCollection collection,
            string attributeName, string attributeValue)
        {

            foreach (ConfigurationElement element in collection)
            {
                if (String.Equals(attributeValue, (string)element[attributeName], StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }

            return null;
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            string result = string.Empty;

            string site_name = HostingEnvironment.SiteName;
            string applicationPath = HostingEnvironment.ApplicationVirtualPath;

            IEnumerable<ConfigurationElement> virtualDirectories =
                GetVirtualDirectories(site_name, applicationPath);

            foreach (ConfigurationElement virtualDirectory in virtualDirectories)
            {
                if ((string)virtualDirectory["path"] == "/") continue;
                string physicalPath = (string)virtualDirectory["physicalPath"];
                var module_dir = new DirectoryInfo(physicalPath);
                if (!module_dir.Exists) continue;
                foreach (FileInfo file in module_dir.GetFiles("*.js", SearchOption.AllDirectories))
                    using (var stream = new StreamReader(file.FullName))
                    {
                        result += "//" + file.FullName;
                        result += Environment.NewLine;
                        result += stream.ReadToEnd();
                        result += Environment.NewLine;
                    }

            }

            response.Write(result);
        }
    }
}
