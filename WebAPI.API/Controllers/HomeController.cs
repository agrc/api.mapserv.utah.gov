using System.Linq;
using System.Web.Mvc;
using WebAPI.API.Commands.Sgid;
using WebAPI.API.Models.ViewModels;
using WebAPI.Common.Executors;

namespace WebAPI.API.Controllers
{
    public class HomeController : Controller
    {
#if !DEBUG
        [OutputCache(Duration=9999)]
#endif
        [HttpGet]
        public ActionResult Index()
        {
            if (App.SgidCategories == null)
            {
                App.SgidCategories = CommandExecutor.ExecuteCommand(new GetSgidCategoriesCommand());
            }

            var categories = App.SgidCategories.Select(item => new SelectListItem
            {
                Text = item,
                Value = item
            }).ToList();

            var versions = new[]
            {
                new SelectListItem
                {
                    Text = "10",
                    Value = "10"
                }
            };

            return View(new MainView()
                            .WithSgidCategories(categories)
                            .WithSgidVersions(versions));
        }
    }
}