using AGRC.api.Models;
using Google.Cloud.Firestore;

namespace AGRC.api.Services;
public class FirestoreApiKeyRepository(FirestoreDb singleton) : IApiKeyRepository {
    private readonly FirestoreDb _db = singleton;

    public async Task<ApiKey> GetKey(string key) {
        var snapshot = await _db.Collection("keys").Document(key.ToLowerInvariant()).GetSnapshotAsync();

        return snapshot.ConvertTo<ApiKey>();
    }
}
