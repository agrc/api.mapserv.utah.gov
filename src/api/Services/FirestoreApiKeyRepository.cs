using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Models;
using AGRC.api.Models.Linkables;
using Google.Cloud.Firestore;

namespace AGRC.api.Services {
    public class FirestoreApiKeyRepository : IApiKeyRepository, ICacheRepository {
        private readonly Dictionary<string, decimal[]> _exclusions = new() {
            {"bryce canyon", new[] {397995.510155659M, 4170226.5544169028M}},
            {"alta", new[] {446372M, 4493558M}},
            {"big water", new[] {440947.64679022622M, 4103827.0225703362M}},
            {"boulder", new[] {462787.3238216745M, 4195803.554941956M}}
        };

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

        public async Task<IEnumerable<PlaceGridLink>> GetPlaceNames() {
            return Enumerable.Empty<PlaceGridLink>();
        }

        public async Task<IEnumerable<ZipGridLink>> GetZipCodes() {
            return Enumerable.Empty<ZipGridLink>();
        }

        public async Task<IEnumerable<UspsDeliveryPointLink>> GetDeliveryPoints() {
            return Enumerable.Empty<UspsDeliveryPointLink>();
        }

        public async Task<IDictionary<int, PoBoxAddress>> GetPoBoxes() {
            return new Dictionary<int, PoBoxAddress>(0);
        }

        public async Task<IEnumerable<PoBoxAddressCorrection>> GetCorrections() {
            return Enumerable.Empty<PoBoxAddressCorrection>();
        }
    }
}
