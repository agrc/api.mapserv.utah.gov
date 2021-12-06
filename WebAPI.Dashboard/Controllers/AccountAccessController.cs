using System.Linq;
using System.Web.Mvc;
using Ninject;
using Raven.Client.Documents;
using WebAPI.Common.Authentication.Forms;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Commands.Email;
using WebAPI.Dashboard.Commands.Key;
using WebAPI.Dashboard.Commands.Password;
using WebAPI.Dashboard.Models.ViewModels;
using WebAPI.Dashboard.Models.ViewModels.Passwords;
using WebAPI.Dashboard.Models.ViewModels.Users;

namespace WebAPI.Dashboard.Controllers
{
    public class AccountAccessController : RavenController
    {
        public AccountAccessController(IDocumentStore store) : base(store) {}

        [Inject]
        public IFormsAuthentication FormsAuthentication { get; set; }

#if !DEBUG
        [OutputCache(Duration=9999)]
#endif
        [HttpGet]
        public ViewResult Index()
        {
            return View(new MainViewModel(Account));
        }

        [HttpPost]
        public ActionResult Login(LoginCredentials credentials, string returnUrl)
        {
            var account = Account;
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Email address is not valid.";

                return View("Index", new MainViewModel(account).WithAccountAccessBase(credentials));
            }

            ValidateLoginCredentials storedCredentials;
            using (Session)
            {
                storedCredentials = Session.Query<Account, IndexEmail>()
                                           .Where(x => x.Email == credentials.Email)
                                           .AsEnumerable()
                                           .Select(x => new
                                               ValidateLoginCredentials(x.Password.HashedPassword, x.Password.Salt,
                                                                            x.Id))
                                           .SingleOrDefault();
            }

            if (storedCredentials == null)
            {
                ErrorMessage = string.Format("Your password does not match. <a href='{0}'>{1}</a> your password.",
                                             Url.Action("PasswordReset", "AccountAccess", new
                                             {
                                                 email = credentials.Email
                                             }, "http"),
                                             "Reset");

                return View("Index", new MainViewModel(account)
                    .WithAccountAccessBase(credentials));
            }

            var task =
                CommandExecutor.ExecuteCommand(new ValidateUserPasswordCommand(credentials.Password, storedCredentials));

            if (!task.Result)
            {
                ErrorMessage = string.Format("Your password does not match. <a href='{0}'>{1}</a> your password.",
                                             Url.Action("PasswordReset", "AccountAccess", new
                                             {
                                                 email = credentials.Email
                                             }, "http"),
                                             "Reset");

                return View("Index", new MainViewModel(account)
                    .WithAccountAccessBase(credentials));
            }
            
            FormsAuthentication.SignIn(storedCredentials.Id);
            App.ResetOutputCache();

            ActionResult redirect;
            if (RedirectToReturnUrl(returnUrl, out redirect))
            {
                return redirect;
            }

            return RedirectToRoute("secure", new
            {
                Controller = "Home"
            });
        }

        [HttpPost]
        public ActionResult Register(LoginCredentials registrationData)
        {
            var account = Account;

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Email address is not valid.";

                return View("Index", new MainViewModel(account)
                    .WithAccountAccessBase(registrationData));
            }

            if (Session.Query<Account, IndexEmail>()
                       .Any(x => x.Email == registrationData.Email))
            {
                ErrorMessage = "Email address is already in use.";

                return View("Index", new MainViewModel(account)
                    .WithAccountAccessBase(registrationData));
            }

            var task = CommandExecutor.ExecuteCommand(new HashPasswordCommand(registrationData.Password));
            var confirmationKey = CommandExecutor.ExecuteCommand(new GenerateUniqueConfirmationKeyCommand(Session));

            var newAccount = new Account
            {
                Email = registrationData.Email,
                Password = task.Result,
                Confirmation = new EmailConfirmation(confirmationKey),
                KeyQuota = new KeyQuota(App.KeySoftLimit)
            };

            Session.Store(newAccount);

            FormsAuthentication.SignIn(newAccount.Id);

            var url = Url.Action("Confirm", "Profile", new
            {
                newAccount.Confirmation.Key
            },
                                 "http");

            CommandExecutor.ExecuteCommand(new SendConfirmationEmailCommand(newAccount, url));

            return RedirectToRoute("secure", new
            {
                Controller = "Profile"
            });
        }

#if !DEBUG
        [OutputCache(Duration=9999)]
#endif
        [HttpGet]
        public ViewResult PasswordReset(PasswordResetCredentials user)
        {
            return View(new MainViewModel(Account).WithUser(new Account
            {
                Email = user.Email
            }));
        }

        [HttpPost]
        public RedirectToRouteResult Reset(PasswordResetCredentials user)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.FlattenErrors();

                return RedirectToAction("PasswordReset");
            }

            var account = Session.Query<Account, IndexEmail>()
                                 .SingleOrDefault(x => x.Email == user.Email);

            if (account == null)
            {
                ErrorMessage = "Email address not found.";

                return RedirectToAction("PasswordReset");
            }

            var passwordToHash = CommandExecutor.ExecuteCommand(new GeneratePasswordCommand());
            var task = CommandExecutor.ExecuteCommand(new HashPasswordCommand(passwordToHash));

            account.Password = task.Result;

            CommandExecutor.ExecuteCommand(new SendPasswordResetEmailCommand(user.Email, passwordToHash));

            Message = "Your password was reset. Please check your email and login.";

            return RedirectToAction("Index", "AccountAccess");
        }

        [NonAction]
        private bool RedirectToReturnUrl(string returnUrl, out ActionResult redirect)
        {
            redirect = null;

            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl) &&
                returnUrl.Length > 1 &&
                returnUrl.StartsWith("/") &&
                !returnUrl.StartsWith("//") &&
                !returnUrl.StartsWith("/\\"))
            {
                redirect = Redirect(returnUrl);
                return true;
            }

            return false;
        }
    }
}