using Google.Cloud.Firestore;
using ugrc.api.Models;

namespace ugrc.api.Services;
public class FirestoreApiKeyRepository(FirestoreDb singleton) : IApiKeyRepository {
    private readonly FirestoreDb _db = singleton;

    public async Task<ApiKey> GetKey(string key) {
        var snapshot = await _db.Collection("keys").Document(key.ToLowerInvariant()).GetSnapshotAsync();

        return snapshot.ConvertTo<ApiKey>();
    }
}
