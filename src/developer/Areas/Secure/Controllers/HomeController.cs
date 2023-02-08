using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using developer.mapserv.utah.gov.Areas.Secure.Models.Database;
using developer.mapserv.utah.gov.Areas.Secure.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace developer.mapserv.utah.gov.Areas.Secure.Controllers {
    [Authorize]
    [Area("secure")]
    [Route("~/secure")]
    public class HomeController : Controller {
        public NpgsqlConnection Connection { get; }

        public HomeController(NpgsqlConnection connection) {
            Connection = connection;
        }

        [HttpGet]
        [Route("")]
        public async Task<ViewResult> Index() {
            // TODO: Refactor
            var idString = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var id = -1;

            if (!string.IsNullOrEmpty(idString)) {
                id = Convert.ToInt32(idString);
            }

            var count = await Connection.QueryFirstOrDefaultAsync<int>(
                @"SELECT COUNT(apikeys.id)
                FROM public.apikeys
			    WHERE account_id = @id
                AND deleted = @deleted",
                new {
                    deleted = false,
                    id
                });

            return View(new IndexViewModel(null, count));
        }

        [HttpGet]
        [Route("keymanagement")]
        public async Task<ViewResult> KeyManagement() {
            // TODO: Refactor
            var idString = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var id = -1;

            if (!string.IsNullOrEmpty(idString)) {
                id = Convert.ToInt32(idString);
            }

            var confirmed = await Connection.QueryFirstOrDefaultAsync<bool>("SELECT email_confirmed from public.accounts where id = @id",
                                                                       new {
                                                                           id
                                                                       });

            if (!confirmed) {
                var profileLink = Url.RouteUrl(new {
                    action = "index",
                    controller = "profile",
                    area = "secure"
                });

                ViewData["Error"] = "Your email has not been confirmed. While you <em>can</em> create API keys, they will <strong>not</strong> authorize access. " +
                    $"Browse to your <a href='{profileLink}'>profile</a> and click the confirm button next to your email and " +
                     "follow the instructions in your inbox.";
            }

            var quota = await Connection.QueryFirstOrDefaultAsync<KeyQuotaDTO>("SELECT keys_used as keysused, keys_allowed as keysallowed FROM public.accounts WHERE id = @id",
                                                                 new {
                                                                     id
                                                                 });

            var keys = await Connection.QueryAsync<ApiKeyDTO>(
                @"SELECT id, key, notes, elevated, enabled, type, deleted, configuration, created_at_ticks AS createdatticks, pattern
				FROM public.apikeys
				WHERE account_id = @id",
                                                                            new {
                                                                                id
                                                                            });

            return View(new KeyManagementViewModel(quota, keys));
        }

        [HttpGet]
        [Route("generatekey")]
        public ViewResult GenerateKey() => View();
    }
}
