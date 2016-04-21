using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            if (!request.Properties.Any())
            {
                //properties is null under test need to add basic configuration
                request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            }

            var apikey = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");
            return base.SendAsync(request, cancellationToken).ContinueWith(
                response => LogResponse(apikey, response));
        }

        private HttpResponseMessage LogResponse(string apikey,
                                                Task<HttpResponseMessage> response)
        {
            var db = Redis.GetDatabase();

            db.StringIncrement(apikey, flags: CommandFlags.FireAndForget);
            db.StringSet(apikey + ":time", DateTime.UtcNow.Ticks, flags: CommandFlags.FireAndForget);

            return response.Result;
        }
    }
}