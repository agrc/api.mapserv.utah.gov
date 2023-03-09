using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGRC.api.Features.Health {
    public class KeyStoreHealthCheck : IHealthCheck {
        private readonly FirestoreDb _db;

        public KeyStoreHealthCheck(FirestoreDb singleton) {
            _db = singleton;
        }
        public string Name => nameof(KeyStoreHealthCheck);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            var stopWatch = Stopwatch.StartNew();
            var collections = new List<string>();

            try {
                await foreach (var collection in _db.ListRootCollectionsAsync()) {
                    collections.Add(collection.Id);
                }
            } catch (Exception ex) {
                return HealthCheckResult.Unhealthy("Unable to access key store", ex, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    }
                );
            }

            if (!collections.Contains("keys")) {
                return HealthCheckResult.Unhealthy("Key store is missing required collections", null, new Dictionary<string, object> {
                        { "duration", stopWatch.ElapsedMilliseconds }
                    }
                );
            }

            return HealthCheckResult.Healthy("key store ready", new Dictionary<string, object> {
                { "duration", stopWatch.ElapsedMilliseconds }
            });
        }
    }
}
