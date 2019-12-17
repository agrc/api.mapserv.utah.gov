using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Ninject;
using Serilog;
using StackExchange.Redis;
using WebAPI.Common.Providers;

namespace WebAPI.API.Handlers.Delegating
{
    public class ApiLoggingHandler : DelegatingHandler
    {
        [Inject]
        public ConnectionMultiplexer Redis { get; set; }

        [Inject]
        public RouteDataProvider RouteDataProvider { get; set; }

        [Inject]
        public HttpContentProvider HttpContentProvider { get; set; }

        [Inject]
        public ApiKeyProvider ApiKeyProvider { get; set; }

        public static DateTime Today
        {
            get { return DateTime.Today.AddHours(20); }
        }

        public static DateTime Month
        {
            get
            {
                var daysIntoMonth = DateTime.Today.Day - 1;
                var timeSpan = new TimeSpan(daysIntoMonth, 4, 0, 0);

                return DateTime.Today.AddMonths(1) - timeSpan;
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Properties.Any())
            {
                //properties is null under test need to add basic configuration
                request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            }

            var apikey = await ApiKeyProvider.GetApiFromRequestAsync(request);

            return await base.SendAsync(request, cancellationToken).ContinueWith(
                response => LogResponse(apikey, response, request), cancellationToken);
        }

        private HttpResponseMessage LogResponse(string apikey, Task<HttpResponseMessage> response, HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(apikey))
            {
                return response.Result;
            }

            var db = Redis.GetDatabase();

            db.StringIncrement(apikey, flags: CommandFlags.FireAndForget);
            db.StringSet(apikey + ":time", DateTime.UtcNow.Ticks, flags: CommandFlags.FireAndForget);

            db.StringIncrement(apikey + ":today", flags: CommandFlags.FireAndForget);
            db.KeyExpire(apikey + ":today", CalculateTimeUntil(DateTime.Now, Today));

            db.StringIncrement(apikey + ":month", flags: CommandFlags.FireAndForget);
            db.KeyExpire(apikey + ":month", CalculateTimeUntil(DateTime.Now, Month));

            db.StringIncrement(apikey + ":minute", flags: CommandFlags.FireAndForget);
            var ttl = db.KeyTimeToLive(apikey + ":minute");
            if (!ttl.HasValue || ttl.Value == TimeSpan.MinValue)
            {
                db.KeyExpire(apikey + ":minute", TimeSpan.FromMinutes(1));
            }

            var routeData = RouteDataProvider.GetRouteData(request);

            if (string.IsNullOrEmpty(routeData.Controller) ||
                string.IsNullOrEmpty(routeData.Action) ||
                string.IsNullOrEmpty(apikey))
            {
                return response.Result;
            }

            switch (routeData.Controller.ToUpper())
            {
                case "GEOCODE":
                {
                    LogRequest("geocode", apikey, db);
                    break;
                }
                case "INFO":
                {
                    LogRequest("info", apikey, db);
                    break;
                }
                case "SEARCH":
                {
                    LogRequest("search", apikey, db);
                    break;
                }
            }

            return response.Result;
        }

        private static void LogRequest(string type, string key, IDatabase db)
        {
            Log.Warning("api({type}): {key}", type.ToLower(), key);
            db.StringIncrement(string.Format("{0}:{1}", key, type), flags: CommandFlags.FireAndForget);
        }

        public static TimeSpan CalculateTimeUntil(DateTime currentTime, DateTime specifiedTime)
        {
            return specifiedTime - currentTime;
        }
    }
}