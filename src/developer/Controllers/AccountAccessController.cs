using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using developer.mapserv.utah.gov.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using developer.mapserv.utah.gov.Models.SecretOptions;
using developer.mapserv.utah.gov.Models.Database;
using System.Security.Cryptography;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace developer.mapserv.utah.gov.Controllers {
    [Route("accountaccess")]
    public class AccountAccessController : Controller {
        public AccountAccessController(NpgsqlConnection connection, IOptions<PepperModel> options) {
            Connection = connection;
            Pepper = options.Value.Pepper;
        }

        public NpgsqlConnection Connection { get; }
        public string Pepper { get; }

        [HttpGet]
        [Route("")]
        public ViewResult Index() => View();

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(LoginViewModel credentials) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var count = await Connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(id) FROM public.accounts WHERE email = @email", new {
                email = credentials.Email
            });

            if (count != 0) {
                return BadRequest(ModelState);
            }

            var saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(saltBytes);
            }

            var salt = Convert.ToBase64String(saltBytes);
            var hash = HashPassword(credentials, saltBytes);

            var rand = new Random(DateTime.Now.Millisecond);
            var guid = Guid.NewGuid().ToString();
            var alpha = guid.Substring(0, guid.IndexOf("-", StringComparison.Ordinal)).ToUpper();

            var confirmationCode = $"AGRC-{alpha}{rand.Next(100000, 999999).ToString(CultureInfo.InvariantCulture)}";

            var id = await Connection.QuerySingleAsync<int>(@"INSERT INTO public.accounts (email, email_key, salt, password)
            						 VALUES (@email, @confirmationCode, @salt, @hash)
									 RETURNING id", new {
                credentials.Email,
                confirmationCode,
                salt,
                hash
            });

            await SignIn(credentials, id);

            // TODO send email

            return RedirectToRoute(new {
                area = "secure",
                controller = "profile",
                action = "index",
            });
        }

        private string HashPassword(LoginViewModel credentials, byte[] saltBytes) => Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: Pepper + credentials.Password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginViewModel credentials, string returnUrl) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var resetUrl = Url.Action("PasswordReset", "AccountAccess", new {
                email = credentials.Email
            }, "http");
            var resetMessage = $"Your password does not match. <a href='{resetUrl}'>Reset</a> your password.";

            var userData = await Connection.QueryFirstOrDefaultAsync<AuthenticationDTO>("SELECT id, salt, password FROM public.accounts WHERE email = @email", new {
                email = credentials.Email
            });

            if (!userData.IsValid()) {
                ViewData.Add("Error", resetMessage);

                return View("Index");
            }

            var hash = HashPassword(credentials, userData.GetSaltBytes(userData.Salt));

            if (hash != userData.Password) {
                ViewData.Add("Error", resetMessage);

                return View("Index");
            }

            await SignIn(credentials, userData.Id);

            return RedirectToRoute(new {
                area = "secure",
                controller = "home",
                action = "index"
            });
        }

        private async Task SignIn(LoginViewModel credentials, int id) {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, credentials.Email),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                //new Claim(ClaimTypes.Name, ""),
                //new Claim(ClaimTypes.Role, "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                         new ClaimsPrincipal(claimsIdentity),
                                         authProperties);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<RedirectToRouteResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToRoute(new {
                action = "index",
                controller = "accountaccess"
            });
        }
    }
}
