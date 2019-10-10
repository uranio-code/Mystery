using Mystery.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{
    public class AutoDocRoutes : IMysteryWebRouteProvider
    {
        public IEnumerable<MysteryWebRoute> getRoutes()
        {
            var result = new List<MysteryWebRoute>();
            result.Add(new MysteryWebRoute
            {
                when = "/autodoc",
                controller = "MysteryAutoDocController",
                templateUrl = "MysteryWebContent/AutoDocControls/AutoDocView.html"
            });
            return result;
        }
    }
}
