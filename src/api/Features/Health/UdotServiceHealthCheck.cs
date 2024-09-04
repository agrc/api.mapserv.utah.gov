using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using ugrc.api.Formatters;
using ugrc.api.Models.ArcGis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ugrc.api.Features.Health;
public class UdotServiceHealthCheck(IHttpClientFactory factory, ILogger log) : IHealthCheck {
    private const string Url = "/server/rest/services/LrsEnabled/Read_Only_Public_LRS_Routes/MapServer/exts/LRServer/networkLayers/1?f=json";
    private readonly HttpClient _client = factory.CreateClient("udot");
    private readonly MediaTypeFormatter[] _mediaTypes = [new TextPlainResponseFormatter()];
    private readonly ILogger? _log = log?.ForContext<LocatorHealthCheck>();

    public string Name => nameof(UdotServiceHealthCheck);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var stopWatch = Stopwatch.StartNew();
        try {
            var message = await _client.GetAsync(Url, cancellationToken);
            var result = await message.Content.ReadAsAsync<ServiceInformation>(_mediaTypes, cancellationToken);

            if (!result.IsSuccessful) {
                _log?.Warning("Unable to access Roads and Highways");

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
