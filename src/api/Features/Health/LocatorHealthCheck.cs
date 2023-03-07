using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;

namespace AGRC.api.Features.Health {
    public class LocatorHealthCheck : IHealthCheck {
        public string Name => nameof(LocatorHealthCheck);

        private readonly HttpClient _client;
        private readonly MediaTypeFormatter[] _mediaTypes;
        private readonly List<LocatorConfiguration> _locatorMetadata;
        private readonly ILogger _log;

        public LocatorHealthCheck(IOptions<List<LocatorConfiguration>> options, IHttpClientFactory factory, ILogger log) {
            _client = factory.CreateClient("health-check");
            _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };

            _locatorMetadata = options.Value;
            _log = log?.ForContext<LocatorHealthCheck>();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            var results = new Dictionary<string, HealthCheckResult>(_locatorMetadata.Count);

            foreach (var locator in _locatorMetadata) {
                var stopWatch = Stopwatch.StartNew();
                try {
                    var message = await _client.GetAsync($"{locator.Url()}{locator.ServiceName}/GeocodeServer?f=json", cancellationToken);
                    var result = await message.Content.ReadAsAsync<LocatorServiceStatus>(_mediaTypes, cancellationToken);

                    if (!result.IsSuccessful) {
                        _log.ForContext("locator", locator)
                            .Warning("unsuccessful health check {service}", locator.ServiceName);

                        results.Add(locator.ServiceName, HealthCheckResult.Degraded("Unable to access geocode service", null, new Dictionary<string, object> {
                            { "duration", stopWatch.ElapsedMilliseconds }
                        }));

                        continue;
                    }
                } catch (Exception ex) {
                    results.Add(locator.ServiceName, HealthCheckResult.Degraded("Unable to access geocode service", ex, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    }));

                    _log.ForContext("locator", locator)
                        .Warning("health check failed {service}", locator.ServiceName);

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
