using System.Web.Mvc;

namespace WebAPI.Dashboard.Controllers
{
    public class HomeController : Controller
    {
#if !DEBUG
        [OutputCache(Duration=9999)]
#endif
        [HttpGet]
        public RedirectToRouteResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("secure", new
                {
                    Controller = "Home"
                });
            }

            return RedirectToRoute("Default", new
            {
                Controller = "AccountAccess"
            });
        }
    }
}