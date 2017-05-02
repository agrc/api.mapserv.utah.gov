using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Dashboard.Controllers;

namespace WebAPI.Dashboard.Areas.admin.Controllers
{
    [Authorize]
    public class HomeController : RavenController
    {
        public HomeController(IDocumentStore store)
            : base(store)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!Session.Query<AdminContainer>().Any(x => x.Emails.Any(y => y == Account.Email)))
            {
                ErrorMessage = "Nothing to see there.";

                return RedirectToRoute("default", new
                    {
                        controller = "Home"
                    });
            }


            return View("Index");
        }
    }
}