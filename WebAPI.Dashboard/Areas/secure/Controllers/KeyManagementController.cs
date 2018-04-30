using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ninject;
using Raven.Client;
using Raven.Client.Linq;
using StackExchange.Redis;
using WebAPI.Common.ActionResults;
using WebAPI.Common.Executors;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Commands.Key;
using WebAPI.Dashboard.Controllers;
using WebAPI.Dashboard.Models.ViewModels;
using WebAPI.Dashboard.Queries;

namespace WebAPI.Dashboard.Areas.secure.Controllers
{
    /// <summary>
    ///     Controller for managing api keys
    /// </summary>
    [Authorize]
    public class KeyManagementController : RavenController
    {
        public KeyManagementController(IDocumentStore store)
            : base(store) {}

        [Inject]
        public ConnectionMultiplexer Redis { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            var account = Account;

            var keys = Session.Query<ApiKey, IndexKeysForUser>()
                               .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                               .Where(x => !x.Deleted && x.AccountId == Account.Id);

            var stats = CommandExecutor.ExecuteCommand(new GetBasicUsageStatsCommand(Redis.GetDatabase(), keys));

            if (account.KeyQuota.KeysUsed == 0)
            {
                var href = Url.Action("Index", new
                {
                    Controller = "GenerateKey"
                });

                Message =
                    "You haven't created any API keys yet. To start using the web API " +
                    $"click <a href='{href}'>Generate</a>.";
            }

            return View(new MainViewModel(account)
                .WithAllUserKeys(stats)
                .WithKeyQuota(account.KeyQuota));
        }

        [HttpDelete]
        public JsonNetResult DeleteKey(string key)
        {
            var account = Account;

            ApiKey apiKeyInfo;
            try
            {
                apiKeyInfo = Session.Query<ApiKey, IndexApiKey>()
                                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                                    .Single(x => x.Key == key);
            } catch (ArgumentNullException ex)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>($"Key, {key}, was not found. {ex.Message}",
                                                 HttpStatusCode.NotFound));
            } catch (InvalidOperationException ex)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>($"Key, {key}, was not found. {ex.Message}",
                                                 HttpStatusCode.NotFound));
            }

            if (apiKeyInfo == null)
            {
                return
                    new JsonNetResult(new JsonResponse<string>($"Key, {key}, was not found.",
                                                               HttpStatusCode.NotFound));
            }

            if (apiKeyInfo.AccountId != account.Id)
            {
                return
                    new JsonNetResult(new JsonResponse<string>($"Key, {key}, is owned by another user.",
                                          HttpStatusCode.Forbidden));
            }

            apiKeyInfo.Deleted = true;
            Session.SaveChanges();

            account.KeyQuota.KeysUsed = CommandExecutor.ExecuteCommand(new CountApiInfosForUserQuery(Session, account));

            return new JsonNetResult(new JsonResponse<string>("", HttpStatusCode.OK));
        }

        [HttpPut]
        public JsonNetResult PauseKey(string key)
        {
            var account = Account;
            ApiKey apiKeyInfo;
            try
            {
                apiKeyInfo = Session.Query<ApiKey, IndexApiKey>()
                                    .Single(x => x.Key == key);
            } catch (ArgumentNullException ex)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>($"Key, {key}, was not found. {ex.Message}",
                                                 HttpStatusCode.NotFound));
            } catch (InvalidOperationException ex)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>($"Key, {key}, was not found. {ex.Message}",
                                                 HttpStatusCode.NotFound));
            }

            if (apiKeyInfo == null)
            {
                return
                    new JsonNetResult(new JsonResponse<string>($"Key, {key}, was not found.",
                                                               HttpStatusCode.NotFound));
            }

            if (apiKeyInfo.AccountId != account.Id)
            {
                return
                    new JsonNetResult(new JsonResponse<string>($"Key, {key}, is owned by another user.",
                                          HttpStatusCode.Forbidden));
            }

            apiKeyInfo.ApiKeyStatus = ApiKey.KeyStatus.Disabled;
            Session.SaveChanges();

            account.KeyQuota.KeysUsed = CommandExecutor.ExecuteCommand(new CountApiInfosForUserQuery(Session, account));

            return new JsonNetResult(new JsonResponse<string>("", HttpStatusCode.OK));
        }

        [HttpPut]
        public JsonNetResult PlayKey(string key)
        {
            var account = Account;
            if (account.KeyQuota.KeysAllowed - account.KeyQuota.KeysUsed <= 0)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>(
                            "You have reached the API key quota. Please delete or deactivate keys that aren't in use or contact AGRC to increase your quota.",
                            HttpStatusCode.Forbidden));
            }

            ApiKey apiKeyInfo;
            try
            {
                apiKeyInfo = Session.Query<ApiKey, IndexApiKey>()
                                    .Single(x => x.Key == key);
            } catch (ArgumentNullException ex)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>($"Key, {key}, was not found. {ex.Message}",
                                                 HttpStatusCode.NotFound));
            } catch (InvalidOperationException ex)
            {
                return
                    new JsonNetResult(
                        new JsonResponse<string>($"Key, {key}, was not found. {ex.Message}",
                                                 HttpStatusCode.NotFound));
            }

            if (apiKeyInfo == null)
            {
                return
                    new JsonNetResult(new JsonResponse<string>($"Key, {key}, was not found.",
                                                               HttpStatusCode.NotFound));
            }

            if (apiKeyInfo.AccountId != account.Id)
            {
                return
                    new JsonNetResult(new JsonResponse<string>($"Key, {key}, is owned by another user.",
                                          HttpStatusCode.Forbidden));
            }

            apiKeyInfo.ApiKeyStatus = ApiKey.KeyStatus.Active;
            Session.SaveChanges();

            account.KeyQuota.KeysUsed = CommandExecutor.ExecuteCommand(new CountApiInfosForUserQuery(Session, account));

            return new JsonNetResult(new JsonResponse<string>("", HttpStatusCode.OK));
        }
    }
}
