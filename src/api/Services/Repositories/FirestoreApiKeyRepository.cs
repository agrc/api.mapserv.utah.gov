using AGRC.api.Models;
using Google.Cloud.Firestore;

#nullable enable
namespace AGRC.api.Services;
public class FirestoreApiKeyRepository : IApiKeyRepository {
    private readonly FirestoreDb _db;

    public FirestoreApiKeyRepository(FirestoreDb singleton) {
        _db = singleton;
    }

    public async Task<ApiKey> GetKey(string key) {
        key = key.ToLowerInvariant();

        var reference = _db.Collection("keys").Document(key);
        var snapshot = await reference.GetSnapshotAsync();

        return snapshot.ConvertTo<ApiKey>();
    }
}
