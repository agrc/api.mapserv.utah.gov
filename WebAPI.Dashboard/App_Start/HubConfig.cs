using System.Web.Routing;

namespace WebAPI.Dashboard
{
    public static class HubConfig
    {
        public static void RegisterHubs(RouteCollection routes)
        {
            //must be first or 404's
            routes.MapHubs();
        }
    }
}