using Google.Cloud.Firestore;
using ugrc.api.Models;
using ZiggyCreatures.Caching.Fusion;

namespace ugrc.api.Services;
public class FirestoreApiKeyRepository(FirestoreDb singleton, IFusionCacheProvider cacheProvider) : IApiKeyRepository {
    private readonly FirestoreDb _db = singleton;
    private readonly IFusionCache _cache = cacheProvider.GetCache("firestore");

    public async Task<ApiKey> GetKey(string key) => await _cache.GetOrSetAsync<ApiKey>($"key/{key}", async (context, cancellation) => {
        var snapshot = await _db.Collection("keys").Document(key.ToLowerInvariant()).GetSnapshotAsync(cancellation);

        return snapshot.ConvertTo<ApiKey>();
    });
}
