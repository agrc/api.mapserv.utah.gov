using Microsoft.Extensions.Diagnostics.HealthChecks;
using ugrc.api.Features.Health;

namespace api.tests.Features.Health;

public class ApiLivenessHealthCheckTests {
    [Fact]
    public async Task Should_remain_unhealthy_after_instance_failure() {
        var check = new ApiLivenessHealthCheck();
        var context = new HealthCheckContext();

        (await check.CheckHealthAsync(context)).Status.ShouldBe(HealthStatus.Healthy);

        check.MarkUnhealthy();

        (await check.CheckHealthAsync(context)).Status.ShouldBe(HealthStatus.Unhealthy);
        (await check.CheckHealthAsync(context)).Status.ShouldBe(HealthStatus.Unhealthy);
    }
}
