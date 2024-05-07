using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace ugrc.api.Features.Health;
public class CacheHealthCheck(Lazy<IConnectionMultiplexer> redis) : IHealthCheck {
    private readonly IDatabase _db = redis.Value.GetDatabase();

    public string Name => nameof(CacheHealthCheck);

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext _, CancellationToken token = default) {
        var stopWatch = Stopwatch.StartNew();
        try {
            _db.StringIncrement("health");
            _db.StringGet("health");
        } catch (Exception ex) {
            return Task.FromResult(HealthCheckResult.Degraded("Unable to access redis cache", ex, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                }
            ));
        }

        return Task.FromResult(HealthCheckResult.Healthy("cache ready", new Dictionary<string, object> {
            { "duration", stopWatch.ElapsedMilliseconds }
        }));
    }
}
