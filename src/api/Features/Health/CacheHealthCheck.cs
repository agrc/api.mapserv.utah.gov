using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace AGRC.api.Features.Health {
    public class CacheHealthCheck : IHealthCheck {
        private readonly IDatabase _db;

        public CacheHealthCheck(Lazy<IConnectionMultiplexer> redis) {
            _db = redis.Value.GetDatabase();
        }
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
}
