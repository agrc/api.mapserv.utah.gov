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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace developer.mapserv.utah.gov.Controllers
{
    [Route("accountaccess")]
    public class AccountAccessController : Controller
    {
        public AccountAccessController(NpgsqlConnection connection, IOptions<PepperModel> options)
        {
            Connection = connection;
            Pepper = options.Value.Pepper;
        }

        public NpgsqlConnection Connection { get; }
        public string Pepper { get; }

        [HttpGet]
        [Route("")]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginViewModel credentials, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resetUrl = Url.Action("PasswordReset", "AccountAccess", new
            {
                email = credentials.Email
            }, "http");
            var resetMessage = $"Your password does not match. <a href='{resetUrl}'>Reset</a> your password.";

            var items = await Connection.QueryAsync<AuthenticationDTO>("SELECT SALT, PASSWORD FROM public.accounts WHERE EMAIL = @email", new { 
                email = credentials.Email 
            });

            var userData = items.FirstOrDefault();

            if (!userData.IsValid())
            {
                ViewData.Add("Error", resetMessage);

                return View("Index");
            }

            //var salt = new byte[128 / 8];
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(salt);
            //}


            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Pepper + credentials.Password,
                salt: userData.GetSaltBytes(userData.Salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            
            if (hash != userData.Password)
            {
                ViewData.Add("Error", resetMessage);

                return View("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credentials.Email),
                //new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                         new ClaimsPrincipal(claimsIdentity),
                                         authProperties);

            return RedirectToRoute(new
            {
                area = "secure",
                controller = "home",
                action = "index"
            });
        }
    }
}
