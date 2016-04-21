using System.Web.Mvc;

namespace WebAPI.Dashboard.Controllers
{
    public class ChangeLogController : Controller
    {
#if !DEBUG
        [OutputCache(Duration = (60 * 60 * 10), VaryByParam="*")]
#endif
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}