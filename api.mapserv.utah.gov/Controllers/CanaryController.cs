using System;
using System.Diagnostics;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;


namespace api.mapserv.utah.gov.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class CanaryController : Controller
    {
        public CanaryController(IOptions<DbConfiguration> dbOptions) {
            DbOptions = dbOptions;
        }

        public IOptions<DbConfiguration> DbOptions { get; }

        [HttpGet]
        [Route("api/v{version:apiVersion}/canary")]
        public IActionResult Index()
        {
            var connString = $"Host=db;Username=postgres;Password={DbOptions.Value.DbPassword};Database=webapi";

            var stopWatch = Stopwatch.StartNew();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT 1";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message, TimeSpan = stopWatch.ElapsedMilliseconds });
            }

            return Json(new { Success = true, Time = stopWatch.ElapsedMilliseconds });
        }
    }
}
