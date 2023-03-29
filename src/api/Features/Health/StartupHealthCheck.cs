using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGRC.api.Features.Health;
public class StartupHealthCheck : IHealthCheck {
    private volatile bool _isReady;

    public bool StartupCompleted {
        get => _isReady;
        set => _isReady = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext _, CancellationToken cancellationToken = default) {
        if (StartupCompleted) {
            return Task.FromResult(HealthCheckResult.Healthy("ok"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("not ok"));
    }
}
