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

        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default) {
            var stopWatch = Stopwatch.StartNew();
            try {
                var redis = ConnectionMultiplexer.Connect("cache");
                var db = redis.GetDatabase();

                db.StringIncrement("health");
                db.StringGet("health");
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
