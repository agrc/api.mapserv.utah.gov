using System;
using System.Collections.Generic;
using System.Diagnostics;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using StackExchange.Redis;

namespace api.mapserv.utah.gov.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class CanaryController : Controller
    {
        public CanaryController(IOptions<DatabaseConfiguration> dbOptions) {
            DbOptions = dbOptions;
        }

        public IOptions<DatabaseConfiguration> DbOptions { get; }

        [HttpGet]
        [Route("api/v{version:apiVersion}/canary")]
        public ObjectResult Index()
        {
            return Ok(new CanaryResult
            {
                Db = CheckDb(),
                Cache = CheckCache()
            });
        }

        private dynamic CheckDb()
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                using (var conn = new NpgsqlConnection(DbOptions.Value.ConnectionString))
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
                return new { Success = false, ex.Message, TimeSpan = stopWatch.ElapsedMilliseconds };
            }

            return new { Success = true, Time = stopWatch.ElapsedMilliseconds };
        }

        private static dynamic CheckCache() {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                var redis = ConnectionMultiplexer.Connect("cache");
                var db = redis.GetDatabase();

                db.StringIncrement("canary");
                db.StringGet("canary");
            } catch (Exception ex){
                return new { Success = false, ex.Message, TimeSpan = stopWatch.ElapsedMilliseconds };
            }

            return new { Success = true, Time = stopWatch.ElapsedMilliseconds };
        }

        public class CanaryResult
        {
            public dynamic Sql { get; set; }
            public dynamic Cache { get; set; }
            public dynamic Db { get; set; }
            public Dictionary<string, dynamic> Locators { get; set; }
        }
    }
}
