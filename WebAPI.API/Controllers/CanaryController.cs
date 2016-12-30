using System.Web.Mvc;
using WebAPI.API.Commands.Info;
using WebAPI.Common.Executors;

namespace WebAPI.API.Controllers
{
    public class CanaryController : Controller
    {
        [HttpGet]
        public JsonResult Index()
        {
            var result = CommandExecutor.ExecuteCommand(new FlyCanaryCommand());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}