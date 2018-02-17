using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.StartPage
{
    public class StartRouteProvider : IMysteryWebRouteProvider
    {
        public IEnumerable<MysteryWebRoute> getRoutes()
        {
            return new List<MysteryWebRoute>()
            {
                new MysteryWebRoute {
                    when = "/start",
                    controller = "dmsStartPageController as c",
                    templateUrl = "DMS/dms_start_page.html"
                }
            };
        }
    }
}
