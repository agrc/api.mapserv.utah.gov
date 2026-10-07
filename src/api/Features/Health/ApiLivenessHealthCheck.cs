using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ugrc.api.Features.Health;

public class ApiLivenessHealthCheck : IHealthCheck {
    private volatile bool _isUnhealthy;

    public void MarkUnhealthy() => _isUnhealthy = true;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(_isUnhealthy ? HealthCheckResult.Unhealthy("Instance cannot serve requests") : HealthCheckResult.Healthy());
}
