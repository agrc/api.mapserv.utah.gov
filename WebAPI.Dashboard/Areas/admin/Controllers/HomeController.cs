using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using StackExchange.Redis;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Areas.admin.Services;
using WebAPI.Dashboard.Commands.Key;
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


        [HttpGet]
#if !DEBUG
        [OutputCache(Duration=1440)]
#endif
        public ViewResult UserList()
        {
            var accounts = Session.Query<Account>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .ToList()
                .OrderBy(x => x.Email);

            return View("UserList", accounts);
        }

        [HttpGet]
#if !DEBUG
        [OutputCache(Duration=60)]
#endif
        public ActionResult UserStats(string email)
        {
            var account = Session.Query<Account, IndexEmail>()
                                 .SingleOrDefault(x => x.Email == email);

            var keys = Session.Query<ApiKey, IndexKeysForUser>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.AccountId == account.Id)
                .ToList();

            var stats = CommandExecutor.ExecuteCommand(new GetRedisStatsPerKeyCommand(_redis.GetDatabase(), keys))
                .OrderByDescending(x => x.LastUsed);
            
            return View("UserStats", stats);
        }
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