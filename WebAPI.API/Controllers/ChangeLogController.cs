using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebAPI.API.Controllers
{
    public class ChangeLogController : Controller
    {
#if !DEBUG
        [OutputCache(Duration = (60*60*24*7), VaryByParam = "*")]
#endif
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}