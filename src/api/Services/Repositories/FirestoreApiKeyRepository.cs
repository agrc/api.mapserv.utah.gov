using Google.Cloud.Firestore;
using ugrc.api.Features.Health;
using ugrc.api.Models;
using ZiggyCreatures.Caching.Fusion;

namespace ugrc.api.Services;
public class FirestoreApiKeyRepository(FirestoreDb singleton, IFusionCacheProvider cacheProvider, ApiLivenessHealthCheck liveness) : IApiKeyRepository {
    private readonly FirestoreDb _db = singleton;
    private readonly IFusionCache _cache = cacheProvider.GetCache("firestore");

    public async Task<ApiKey> GetKey(string key) {
        return await _cache.GetOrSetAsync<ApiKey>($"key/{key}", async (context, cancellation) => {
            try {
                var snapshot = await _db.Collection("keys").Document(key.ToLowerInvariant()).GetSnapshotAsync(cancellation);

                return snapshot.ConvertTo<ApiKey>();
            } catch (TypeInitializationException exception) when (exception.TypeName == "Google.Cloud.Firestore.V1.Firestore") {
                liveness.MarkUnhealthy();
                throw;
            }
        });
    }
}
