using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using StackExchange.Redis;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Dashboard.Controllers;

namespace WebAPI.Dashboard.Areas.admin.Controllers
{
    [Authorize]
    public class HomeController : RavenController
    {
        private readonly ConnectionMultiplexer _redis;

        public HomeController(IDocumentStore store, ConnectionMultiplexer redis)
            : base(store)
        {
            _redis = redis;
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