using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using developer.mapserv.utah.gov.Models;

namespace developer.mapserv.utah.gov.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [Route("")]
        public RedirectToRouteResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute(new
                {
                    controller = "home",
                    area = "secure"
                });
            }

            return RedirectToRoute(new
            {
                controller = "accountaccess"
            });
        }

        [Route("[action]")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
