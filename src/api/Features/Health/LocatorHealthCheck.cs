using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace AGRC.api.Features.Health {
    public class LocatorHealthCheck : IHealthCheck {
        public string Name => nameof(LocatorHealthCheck);

        private readonly HttpClient _client;
        private readonly MediaTypeFormatter[] _mediaTypes;
        private readonly List<LocatorConfiguration> _locatorMetadata;

        public LocatorHealthCheck(IOptions<List<LocatorConfiguration>> options, IHttpClientFactory factory) {
            _client = factory.CreateClient("default");
            _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };

            _locatorMetadata = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            var results = new Dictionary<string, HealthCheckResult>(_locatorMetadata.Count);

            foreach (var locator in _locatorMetadata) {
                var stopWatch = Stopwatch.StartNew();
                try {
                    var message = await _client.GetAsync($"{locator.Url()}{locator.ServiceName}/GeocodeServer?f=json");
                    var result = await message.Content.ReadAsAsync<LocatorServiceStatus>(_mediaTypes);

                    if (!result.IsSuccessful) {
                        results.Add(locator.ServiceName, HealthCheckResult.Degraded("Unable to access geocode service", null, new Dictionary<string, object> {
                            { "duration", stopWatch.ElapsedMilliseconds }
                        }));

                        continue;
                    }
                } catch (Exception ex) {
                    results.Add(locator.ServiceName, HealthCheckResult.Degraded("Unable to access geocode service", ex, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    }));

                    continue;
                }

                results.Add(locator.ServiceName, HealthCheckResult.Healthy("geocode service ready", new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                }));
            }

            if (results.Values.All(x => x.Status == HealthStatus.Degraded)) {
                return HealthCheckResult.Unhealthy("Unable to access any geocode services");
            }

            if (results.Values.Any(x => x.Status == HealthStatus.Degraded)) {
                return HealthCheckResult.Degraded("Unable to access all geocode services");
            }

            return HealthCheckResult.Healthy("All geocode services ready");
        }
    }
}
