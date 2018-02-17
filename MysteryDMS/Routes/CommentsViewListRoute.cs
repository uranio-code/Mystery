using Mystery.Web;
using System.Collections.Generic;

namespace MysteryDMS.Routes
{
    class CommentsViewListRoute : IMysteryWebRouteProvider
    {
        public IEnumerable<MysteryWebRoute> getRoutes()
        {
            return new List<MysteryWebRoute>()
            {
                new MysteryWebRoute {
                    when = "/DMS",
                    controller = "comments_view_list_controller",
                    templateUrl = "DMS/comments_view_list.html"
                }
            };
        }
    }
}
