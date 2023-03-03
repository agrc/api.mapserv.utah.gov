using System;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Health;
using Microsoft.Extensions.Hosting;

namespace AGRC.api.Services {
    public class CacheHostedService : IHostedService {
        private readonly ILookupCache _cache;

        public CacheHostedService(ILookupCache cache) {
            _cache = cache;
        }
        public async Task StartAsync(CancellationToken token) {
            await _cache.InitializeAsync();

            return;
        }

        public Task StopAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }

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
