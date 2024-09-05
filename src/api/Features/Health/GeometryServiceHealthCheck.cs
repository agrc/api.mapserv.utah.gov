using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using ugrc.api.Features.Geocoding;
using ugrc.api.Formatters;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Features.Health;
public class GeometryServiceHealthCheck(IHttpClientFactory factory, IOptions<GeometryServiceConfiguration> options, ILogger log) : IHealthCheck {
    private const string Url = "/server/rest/services/LrsEnabled/Read_Only_Public_LRS_Routes/MapServer/exts/LRServer/networkLayers/1?f=json";
    private readonly HttpClient _client = factory.CreateClient("health-check");
    private readonly GeometryServiceConfiguration _geometryMetadata = options.Value;
    private readonly MediaTypeFormatter[] _mediaTypes = [new TextPlainResponseFormatter()];
    private readonly ILogger? _log = log?.ForContext<GeometryServiceHealthCheck>();

    public string Name => nameof(GeometryServiceHealthCheck);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var stopWatch = Stopwatch.StartNew();
        try {
            var message = await _client.GetAsync($"{_geometryMetadata.GetHost()}{_geometryMetadata.PathToGeometryServer}?f=json", cancellationToken);
            var result = await message.Content.ReadAsAsync<ServiceInformation>(_mediaTypes, cancellationToken);

            if (!result.IsSuccessful) {
                _log?.Warning("Unable to access GeometryService");

                return HealthCheckResult.Degraded("Unable to geometry service", null, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                });
            }
        } catch (Exception ex) {
            return HealthCheckResult.Degraded("Unable to geometry service", ex, new Dictionary<string, object> {
                { "duration", stopWatch.ElapsedMilliseconds }
            });
        }

        return HealthCheckResult.Healthy("geometry service ready", new Dictionary<string, object> {
            { "duration", stopWatch.ElapsedMilliseconds }
        });
    }
}
