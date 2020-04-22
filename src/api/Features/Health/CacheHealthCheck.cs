using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace api.mapserv.utah.gov.Features.Health {
    public class CacheHealthCheck : IHealthCheck {
        public string Name => nameof(CacheHealthCheck);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            var stopWatch = Stopwatch.StartNew();
            try {
                using var redis = await ConnectionMultiplexer.ConnectAsync("cache");
                var db = redis.GetDatabase();

                db.StringIncrement("health");
                db.StringGet("health");
            } catch (Exception ex) {
                return HealthCheckResult.Degraded("Unable to access redis cache", ex, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    }
                );
            }

            return HealthCheckResult.Healthy("cache ready", new Dictionary<string, object> {
                { "duration", stopWatch.ElapsedMilliseconds }
            });
        }
    }
}
