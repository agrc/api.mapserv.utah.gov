using ugrc.api.Features.Health;
using Microsoft.Extensions.Hosting;

namespace ugrc.api.Services;
public class StartupBackgroundService(StartupHealthCheck healthCheck) : BackgroundService {
    private readonly StartupHealthCheck _healthCheck = healthCheck;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        _healthCheck.StartupCompleted = true;

        return Task.CompletedTask;
    }
}
