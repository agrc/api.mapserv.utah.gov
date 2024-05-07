using System.Diagnostics;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ugrc.api.Features.Health;
public class BigQueryHealthCheck : IHealthCheck {
    public string Name => nameof(BigQueryHealthCheck);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var stopWatch = Stopwatch.StartNew();
        BigQueryClient client;

        try {
            // TODO! switch based on build config or something
            client = await BigQueryClient.CreateAsync("ut-dts-agrc-web-api-dev");
        } catch (Exception ex) {
            return HealthCheckResult.Unhealthy("Unable to access BigQuery", ex, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                }
            );
        }

        try {
            var table = await client.GetTableAsync("address_grid_mapping_cache", "address_system_mapping", cancellationToken: cancellationToken);
        } catch (Exception ex) {
            return HealthCheckResult.Unhealthy("BigQuery table not found", ex, new Dictionary<string, object> {
                    { "duration", stopWatch.ElapsedMilliseconds }
                }
            );
        }

        return HealthCheckResult.Healthy("BigQuery ready", new Dictionary<string, object> {
            { "duration", stopWatch.ElapsedMilliseconds }
        });
    }
}
