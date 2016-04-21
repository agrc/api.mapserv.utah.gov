using System.Web.Mvc;

namespace WebAPI.Dashboard.Areas.admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "admin",
                "admin/{controller}/{action}/{id}",
                new
                {
                    AreaName = "admin",
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new[]
                {
                    "WebAPI.Dashboard.Areas.admin.Controllers"
                }
                );
        }
    }
}