using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;
using StackExchange.Redis;

namespace api.mapserv.utah.gov.Features.Health {
    public class KeyStoreHealthCheck : IHealthCheck {
        private readonly string _connectionString;

        public KeyStoreHealthCheck(IOptions<DatabaseConfiguration> dbOptions) {
            _connectionString = dbOptions.Value.ConnectionString;
        }
        public string Name => nameof(KeyStoreHealthCheck);

        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default) {
            var stopWatch = Stopwatch.StartNew();
            try {
                using (var conn = new NpgsqlConnection(_connectionString)) {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand()) {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT 1";
                        cmd.ExecuteNonQuery();
                    }
                }
            } catch (Exception ex) {
                return Task.FromResult(HealthCheckResult.Unhealthy("Unable to access key store", ex, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    }
                ));
            }

            return Task.FromResult(HealthCheckResult.Healthy("key store ready", new Dictionary<string, object> {
                { "duration", stopWatch.ElapsedMilliseconds }
            }));
        }
    }
}
