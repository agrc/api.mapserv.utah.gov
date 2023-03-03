using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AGRC.api.Features.Health {
    public class CacheHealthCheck : IHealthCheck {
        private readonly string _host;

        public CacheHealthCheck(IOptions<DatabaseConfiguration> redisOptions) {
            _host = redisOptions.Value.Host;
        }
        public string Name => nameof(CacheHealthCheck);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            var stopWatch = Stopwatch.StartNew();
            try {
                using var redis = await ConnectionMultiplexer.ConnectAsync(_host);
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
