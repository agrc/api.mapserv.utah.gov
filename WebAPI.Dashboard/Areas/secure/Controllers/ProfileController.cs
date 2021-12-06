using System;
using System.Linq;
using System.Web.Mvc;
using Raven.Client.Documents;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Areas.secure.Attributes;
using WebAPI.Dashboard.Areas.secure.Models.Enums;
using WebAPI.Dashboard.Commands.Email;
using WebAPI.Dashboard.Commands.Key;
using WebAPI.Dashboard.Commands.Password;
using WebAPI.Dashboard.Controllers;
using WebAPI.Dashboard.Models.ViewModels;
using WebAPI.Dashboard.Models.ViewModels.Passwords;

namespace WebAPI.Dashboard.Areas.secure.Controllers
{
    public class ProfileController : RavenController
    {
        public ProfileController(IDocumentStore store) : base(store) {}

        [HttpGet, Authorize]
        public ActionResult Index()
        {
            if (Account == null)
            {
                ErrorMessage = "User not found.";

                return RedirectToAction("Logout");
            }

            return View(new MainViewModel(Account));
        }

        [HttpPost, Authorize]
        public RedirectToRouteResult UpdateProfile(FormAction formAction)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(Request.Form["Email"]) &&
                Account.Email != Request.Form["Email"] &&
                Account.Confirmation.Confirmed)
            {
                var confirmationKey = CommandExecutor.ExecuteCommand(new GenerateUniqueConfirmationKeyCommand(Session));
                var url = Url.Action("Confirm", "Profile", new
                {
                    Account.Confirmation.Key
                },
                                     "http");

                //set confirmation to defaults
                Account.ResetConfirmation(confirmationKey);
                //send email
                CommandExecutor.ExecuteCommand(new SendConfirmationEmailCommand(Account, url));
            }

            TryUpdateModel(Account, new[]
            {
                "Email", "FirstName", "LastName", "Company", "JobCategory", "JobTitle", "CodingExperience",
                "ContactRoute"
            });
            App.ResetOutputCache();

            switch (formAction)
            {
                case FormAction.Update:
                    Message =
                        string.Format(
                            "You profile was updated. You probably want to go to the <a href='{0}'>Dashboard</a> to manage your API keys.",
                            Url.Action("Index", "Home"));
                    return RedirectToAction("Index");
                case FormAction.UpdateAndGo:
                    return RedirectToAction("Index", "Home", new
                    {
                        Area = "Secure"
                    });
                default:
                    throw new ArgumentOutOfRangeException("formAction");
            }
        }

        [HttpPost, Authorize]
        public JsonResult GenerateConfirmation()
        {
            var url = Url.Action("Confirm", "Profile", new
            {
                Account.Confirmation.Key
            },
                                 "http");

            CommandExecutor.ExecuteCommand(new SendConfirmationEmailCommand(Account, url));

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpGet, ConfirmationAuthorization]
        public RedirectToRouteResult Confirm(string key)
        {
            var account = Session.Query<Account, IndexEmailConfirmationKey>()
                                 .SingleOrDefault(x => x.Confirmation.Key == key);

            if (account == null || account.Id != User.Identity.Name)
            {
                ErrorMessage = "Confirmation Key not found.";
                return RedirectToAction("Index", "Profile", new
                {
                    Area = "secure"
                });
            }

            account.Confirmation.Confirmed = true;
            account.Confirmation.ConfirmationDate = DateTime.UtcNow;

            Message = "Thank you for confirming your email address.";

            return RedirectToAction("Index", "Profile", new
            {
                Area = "secure"
            });
        }

        [HttpPost, Authorize]
        public RedirectToRouteResult UpdatePassword(ChangePasswordCredentials info)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.FlattenErrors();
                return RedirectToAction("Index", new MainViewModel(Account));
            }

            if (info.NewPassword != info.ConfirmPassword)
            {
                ErrorMessage =
                    "The new passwords you entered where not the same. Please pick one and use it for both the new and confirmation password.";
                return RedirectToAction("Index", new MainViewModel(Account));
            }

            var hashedPasswordTask =
                CommandExecutor.ExecuteCommand(new HashPasswordCommand(info.CurrentPassword, Account.Password.Salt));

            if (Account.Password.HashedPassword != hashedPasswordTask.Result.HashedPassword)
            {
                ErrorMessage =
                    "The Current password you entered did not match what we have you currently logging in with.";
                return RedirectToAction("Index", new MainViewModel(Account));
            }

            var newHashedPasswordTask = CommandExecutor.ExecuteCommand(new HashPasswordCommand(info.NewPassword));
            Account.Password = newHashedPasswordTask.Result;

            Message = "Your password has been changed successfully. Please use the new one the next time you login.";

            return RedirectToAction("Index");
        }
    }
}