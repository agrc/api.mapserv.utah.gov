using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Ninject;
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
            db.StringIncrement(string.Format("{0}:{1}", key, type), flags: CommandFlags.FireAndForget);
        }
    }
}