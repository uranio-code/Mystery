using System;
using System.Collections.Generic;
using Mystery.Content;
using Mystery.UI;

namespace Mystery.Routes
{
    [ContentType()]
    public class MysteryWebRoute:BaseContent
    {
        [ContentProperty]
        [PropertyView]
        public string  when { get; set; } 

        [ContentProperty]
        [PropertyView]
        public string templateUrl { get; set; } 

        [ContentProperty]
        [PropertyView]
        public string  controller { get; set; }

        [ContentProperty]
        [PropertyView]
        public string Extra { get; set; }


    }

    public interface IMysteryWebRouteProvider {
        IEnumerable<MysteryWebRoute> getRoutes();
    }


    public class MysteryRoutesProvider : IMysteryWebRouteProvider
    {
        public IEnumerable<MysteryWebRoute> getRoutes()
        {
            var result = new List<MysteryWebRoute>();
            result.Add(new MysteryWebRoute {
                when = "/Services",
                controller = "MysteryServiceViewController",
                templateUrl = "MysteryWebContent/Shared/ServiceView.html"
            });
            result.Add(new MysteryWebRoute
            {
                when = "/TOTPAuthentication",
                controller = "TOTPAuthenticationController",
                templateUrl = "MysteryWebContent/TOTPAuthentication/TOTPAuthentication.html"
            });
            result.Add(new MysteryWebRoute
            {
                when = "/UnAuthorized",
                controller = "MysteryUnAuthorizedViewController",
                templateUrl = "MysteryWebContent/Shared/UnAuthorized.html"
            });
            return result;
        }
    }

}
