using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using StackExchange.Redis;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
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
#if !DEBUG
        [OutputCache(Duration=300)]
#endif
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

            if (StatCache.Usage == null || !StatCache.Usage.Any())
            {
                var totalKeys = Session.Query<ApiKey>().Count();
                var totalUsers = Session.Query<Account>().Count();

                StatCache.Usage = HydrateUsageTimeCache();
                StatCache.Keys = totalKeys;
                StatCache.Users = totalUsers;
            }

            return View("Index", new {
                StatCache.Keys,
                StatCache.Users,
                StatCache.LastUsedKey,
                StatCache.MostUsedKey,
                StatCache.TotalRequests
                }.ToExpando());
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

        [HttpGet]
#if !DEBUG
        [OutputCache(Duration=60)]
#endif
        public ViewResult KeyStats(string key)
        {
            var keyInfo = Session.Query<ApiKey, IndexKeysForUser>()
                .SingleOrDefault(x => x.Key == key);

            if (keyInfo == null)
            {
                return View("empty");
            }

            var email = Session.Load<Account>(keyInfo.AccountId).Email;
                
            var keys = new[]
            {
                keyInfo,
            };

            var stats = CommandExecutor.ExecuteCommand(new GetRedisStatsPerKeyCommand(_redis.GetDatabase(), keys))
                .OrderByDescending(x => x.LastUsed);

            return View("KeyStats", new
            {
                key = stats.Single(),
                email
            }.ToExpando());
        }

        public IReadOnlyCollection<Usage> HydrateUsageTimeCache()
        {
            var usage = new List<Usage>();

            var server = _redis.GetServer("localhost", 6379);
            var _db = _redis.GetDatabase();
            var dbIndex = _db.Database;
            var redisKeys = server.Keys(dbIndex);

            foreach (var key in redisKeys.Where(x => !x.ToString().Contains(":")))
            {
                var use = new Usage
                {
                    Key = key
                };

                var value = _db.StringGet(key);
                if (value.HasValue)
                {
                    use.TotalUsageCount = long.Parse(value);
                }

                value = _db.StringGet("{0}:time".With(key));
                if (value.HasValue)
                {
                    use.LastUsedTicks = long.Parse(value);
                }

                value = _db.StringGet("{0}:today".With(key));
                if (value.HasValue)
                {
                    use.UsageToday = long.Parse(value);
                }

                value = _db.StringGet("{0}:month".With(key));
                if (value.HasValue)
                {
                    use.UsageForMonth = long.Parse(value);
                }

                value = _db.StringGet("{0}:geocode".With(key));
                if (value.HasValue)
                {
                    use.TotalGeocodeUsage = long.Parse(value);
                }

                value = _db.StringGet("{0}:search".With(key));
                if (value.HasValue)
                {
                    use.TotalSearchUsage = long.Parse(value);
                }

                value = _db.StringGet("{0}:info".With(key));
                if (value.HasValue)
                {
                    use.TotalInfoUsage = long.Parse(value);
                }

                usage.Add(use);
            }

            return usage.AsReadOnly();
        }

        public class Usage
        {
            public string Key { get; set; }
            public long LastUsedTicks { get; set; }
            public long TotalUsageCount { get; set; }
            public long UsageToday { get; set; }
            public long UsageForMonth { get; set; }
            public long TotalGeocodeUsage { get; set; }
            public long TotalSearchUsage { get; set; }
            public long TotalInfoUsage { get; set; }
        }
    }
}