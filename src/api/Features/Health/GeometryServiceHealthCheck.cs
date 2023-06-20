using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Features.GeometryService;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace AGRC.api.Features.Health;
public class GeometryServiceHealthCheck(IOptions<GeometryServiceConfiguration> dbOptions, IHttpClientFactory factory) : IHealthCheck {
    private readonly string _url = $"{dbOptions.Value.GetHost()}{dbOptions.Value.Path}?f=json";
    private readonly HttpClient _client = factory.CreateClient("health-check");
    private readonly MediaTypeFormatter[] _mediaTypes = new MediaTypeFormatter[] {
            new TextPlainResponseFormatter()
        };

    public string Name => nameof(GeometryServiceHealthCheck);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var stopWatch = Stopwatch.StartNew();
        try {
            var message = await _client.GetAsync(_url, cancellationToken);
            var result = await message.Content.ReadAsAsync<GeometryServiceInformation>(_mediaTypes, cancellationToken);

            if (!result.IsSuccessful) {
                return HealthCheckResult.Degraded("Unable to access geometry service", null, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                });
            }
        } catch (Exception ex) {
            return HealthCheckResult.Degraded("Unable to access geometry service", ex, new Dictionary<string, object> {
                { "duration", stopWatch.ElapsedMilliseconds }
            });
        }

        return HealthCheckResult.Healthy("geometry service ready", new Dictionary<string, object> {
            { "duration", stopWatch.ElapsedMilliseconds }
        });
    }
}
