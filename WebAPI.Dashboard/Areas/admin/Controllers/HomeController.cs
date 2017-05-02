using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using StackExchange.Redis;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Common.Models.Raven.Users;
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

            var accounts = Session.Query<Account>()
                 .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                 .ToList()
                 .OrderBy(x => x.Email);

            return View("Index", accounts);
        }

        public void GetKeys()
        {
            var server = _redis.GetServer("localhost", 6379);
            var db = _redis.GetDatabase();
            var dbIndex = db.Database;
            var redisKeys = server.Keys(dbIndex);

            foreach (var key in redisKeys)
            {
                var value = db.StringGet(key);
            }
        }
    }
}