﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client.Documents;
using Serilog;
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
using WebAPI.Dashboard.Models.ViewModels.Usage;

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

            HydrateCache();

            return View("Index", new
            {
                StatCache.Keys,
                StatCache.Users,
                StatCache.LastUsedKey,
                StatCache.MostUsedKey,
                StatCache.TotalRequests,
                StatCache.RequestsForMonth,
                StatCache.RequestsForToday,
                StatCache.RequestsPerMinute
            }.ToExpando());
        }

        private void HydrateCache()
        {
            if (StatCache.AllKeys != null && StatCache.AllKeys.Any())
            {
                return;
            }

            const int pageSize = 1024;

            var keys = Session.Query<ApiKey>()
                .Take(pageSize)
                .ToList();

            keys.AddRange(Session.Query<ApiKey>()
                .Skip(pageSize)
                .Take(pageSize)
                .ToList()
            );

            StatCache.AllKeys = keys.ToArray();
            
            Log.Warning("analytics(key-count): {key}", StatCache.AllKeys.Length);

            var totalKeys = Session.Query<ApiKey>().Count();
            var totalUsers = Session.Query<Account>().Count();

            StatCache.Usage = HydrateUsageTimeCache(StatCache.AllKeys);
            StatCache.Keys = totalKeys;
            StatCache.Users = totalUsers;
        }

        [HttpGet]
#if !DEBUG
        [OutputCache(Duration=1440)]
#endif
        public ViewResult UserList()
        {
            var accounts = Session.Query<Account>()
                .Customize(x => x.WaitForNonStaleResults())
                .Take(1024)
                .ToList()
                .OrderByDescending(x => x.KeyQuota.KeysUsed);

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
                .Customize(x => x.WaitForNonStaleResults())
                .Where(x => x.AccountId == account.Id)
                .ToList();

            var stats = CommandExecutor.ExecuteCommand(new GetBasicUsageStatsCommand(_redis.GetDatabase(), keys))
                .OrderByDescending(x => x.LastUsedTicks);

            return View("UserStats", stats);
        }

        [HttpGet]
#if !DEBUG
        [OutputCache(Duration=10)]
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
            var stats = CommandExecutor.ExecuteCommand(new GetAllUsageStatsCommand(_redis.GetDatabase(), keyInfo));

            return View("KeyStats", new
            {
                key = stats,
                email
            }.ToExpando());
        }

        public IReadOnlyCollection<UsageViewModel> HydrateUsageTimeCache(IEnumerable<ApiKey> keys)
        {
            var db = _redis.GetDatabase();
            var usage = keys.Select(key => CommandExecutor.ExecuteCommand(new GetAllUsageStatsCommand(db, key))).ToList();

            return usage.AsReadOnly();
        }
    }
}