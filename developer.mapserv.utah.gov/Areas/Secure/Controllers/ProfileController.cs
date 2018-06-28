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

namespace developer.mapserv.utah.gov.Areas.Secure.Controllers
{
    [Authorize]
    [Area("secure")]
    [Route("secure/profile")]
    public class ProfileController : Controller
    {
        public ProfileController(NpgsqlConnection connection)
        {
            Connection = connection;
        }

        public NpgsqlConnection Connection { get; }

        [Route("")]
        public async Task<ViewResult> Index()
        {
            // TODO: Refactor
            var idString = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            int id = -1;

            if (!string.IsNullOrEmpty(idString))
            {
                id = Convert.ToInt32(idString);
            }

            var entity = await Connection.QueryFirstOrDefaultAsync<ProfileDTO>(
                @"SELECT 
                  	email, 
                  	first_name as first, 
                  	last_name as last, 
                    company, 
                    job_category as jobcategory, 
                    job_title as jobtitle, 
                    experience, 
					email_confirmed as confirmed,
                    contact_route as contactroute
                FROM public.accounts
                WHERE id = @id", new { id }
            );

            var model = new ProfileViewModel(entity);

            return View(model);
        }
    }
}
