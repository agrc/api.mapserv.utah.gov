using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Raven.Client;
using WebAPI.Common.Executors;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Areas.secure.Models.ViewModels;
using WebAPI.Dashboard.Commands.Email;
using WebAPI.Dashboard.Commands.Key;
using WebAPI.Dashboard.Controllers;
using WebAPI.Dashboard.Models.ViewModels;
using WebAPI.Dashboard.Queries;

namespace WebAPI.Dashboard.Areas.secure.Controllers
{
    [Authorize]
    public class GenerateKeyController : RavenController
    {
        public GenerateKeyController(IDocumentStore store) : base(store)
        {
        }

        public ActionResult Index()
        {
            var account = Account;
            if (account.KeyQuota.KeysAllowed - account.KeyQuota.KeysUsed <= 0)
            {
                ErrorMessage =
                    "You have reached the API key quota. Please delete or deactivate keys that aren't in use or contact AGRC to increase your quota.";

                return RedirectToAction("Index", "KeyManagement");
            }

            return View(new MainViewModel(account));
        }

        [HttpPost]
        public ActionResult Generate(ApiKeyData data)
        {
            var account = Account;

            if (account.KeyQuota.KeysAllowed - account.KeyQuota.KeysUsed <= 0)
            {
                ErrorMessage =
                    "You have reached the API key quota. Please delete or deactivate keys that aren't in use or contact AGRC to increase your quota.";

                return RedirectToAction("Index", "KeyManagement");
            }

            var command = new ValidateAndGetKeyTypeCommand(data);
            var type = CommandExecutor.ExecuteCommand(command);

            if (type == ApiKey.ApplicationType.None)
            {
                ErrorMessage = command.ErrorMessage;

                if (!string.IsNullOrEmpty(data.Ip))
                    TempData["ip"] = data.Ip;
                else
                {
                    TempData["url"] = data.UrlPattern;
                }

                return RedirectToAction("Index", "GenerateKey");
            }

            var key = CommandExecutor.ExecuteCommand(new GenerateUniqueApiKeyCommand(Session));
            var pattern = CommandExecutor.ExecuteCommand(new FormatKeyPatternCommand(type, data));
            var apiKey = new ApiKey(key)
                {
                    AccountId = account.Id,
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Type = type,
                    AppStatus = data.AppStatus,
                    Pattern = data.UrlPattern ?? data.Ip,
                    RegexPattern = pattern,
                    CreatedAtTicks = DateTime.UtcNow.Ticks,
                    Deleted = false,
                    Key = key
                };

            Session.Store(apiKey);

            Session.SaveChanges();

            Account.KeyQuota.KeysUsed = CommandExecutor.ExecuteCommand(new CountApiInfosForUserQuery(Session, account));

            Task.Factory.StartNew(() => CommandExecutor.ExecuteCommand(new KeyCreatedEmailCommand(account, apiKey)));

            Message = "Key created successfully.";

            return RedirectToAction("Index", "KeyManagement");
        }
    }
}