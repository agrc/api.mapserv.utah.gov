using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ugrc.api.Features.Health;
public class GridMappingHealthCheck(IMemoryCache cache) : IHealthCheck {
    private readonly IMemoryCache _cache = cache;

    public string Name => nameof(GridMappingHealthCheck);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var stopWatch = Stopwatch.StartNew();


        var attempts = 0;
        while (attempts < 5) {
            try {
                if (_cache.TryGetValue("mapping/places", out var places) && !string.IsNullOrEmpty(places?.ToString())) {
                    return HealthCheckResult.Healthy("Grid map ready", new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds },
                        { "attempts", attempts + 1 }
                    });
                }
            } catch (Exception ex) {
                return HealthCheckResult.Unhealthy("Unable to access grid cache", ex, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds },
                    { "attempts", attempts + 1 }
                });
            }

            await Task.Delay(1000, cancellationToken);
            attempts++;
        }

        return HealthCheckResult.Unhealthy("Grid map not ready after 5 attempts", null, new Dictionary<string, object> {
            { "duration", stopWatch.ElapsedMilliseconds },
            { "attempts", attempts }
        });
    }
}
