using System.Web.Mvc;

namespace WebAPI.Dashboard.Areas.secure
{
    public class SecureAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "secure"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "secure",
                "secure/{controller}/{action}/{id}",
                new
                {
                    AreaName = "secure",
                    contoller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new[]
                {
                    "WebAPI.Dashboard.Areas.secure.Controllers"
                }
                );
        }
    }
}