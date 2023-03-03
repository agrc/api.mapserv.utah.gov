using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Health;
using Microsoft.Extensions.Hosting;

namespace AGRC.api.Services {

    public class StartupBackgroundService : BackgroundService {
        private readonly StartupHealthCheck _healthCheck;

        public StartupBackgroundService(StartupHealthCheck healthCheck) {
            _healthCheck = healthCheck;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            _healthCheck.StartupCompleted = true;

            return Task.CompletedTask;
        }
    }
}
