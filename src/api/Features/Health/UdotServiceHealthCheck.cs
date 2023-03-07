using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGRC.api.Features.Health {
    public class UdotServiceHealthCheck : IHealthCheck {
        private const string url = "/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0?f=json";
        private readonly HttpClient _client;
        private readonly MediaTypeFormatter[] _mediaTypes;

        public UdotServiceHealthCheck(IHttpClientFactory factory) {
            _client = factory.CreateClient("udot");
            _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
        }
        public string Name => nameof(UdotServiceHealthCheck);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            var stopWatch = Stopwatch.StartNew();
            try {
                var message = await _client.GetAsync(url, cancellationToken);
                var result = await message.Content.ReadAsAsync<GeometryServiceInformation>(_mediaTypes, cancellationToken);

                if (!result.IsSuccessful) {
                    return HealthCheckResult.Degraded("Unable to access roads and highways", null, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    });
                }
            } catch (Exception ex) {
                return HealthCheckResult.Degraded("Unable to access roads and highways", ex, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                });
            }

            return HealthCheckResult.Healthy("roads and highways ready", new Dictionary<string, object> {
                { "duration", stopWatch.ElapsedMilliseconds }
            });
        }
    }
}
