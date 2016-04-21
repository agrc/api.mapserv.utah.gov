using System.Web.Mvc;

namespace WebAPI.Dashboard.Controllers
{
    public class StartupGuideController : Controller
    {
        [HttpGet]
#if !DEBUG
        [OutputCache(Duration=9999)]
#endif
        public ViewResult Index()
        {
            return View();
        }
    }
}