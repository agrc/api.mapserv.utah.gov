using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Models;
using AGRC.api.Models.Configuration;
using AGRC.api.Models.Linkables;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AGRC.api.Services {
    public class PostgreApiKeyRepository : IApiKeyRepository, ICacheRepository {
        private const string ApiKeyByKey = @"SELECT key,
                   account_id,
                   elevated,
                   enabled,
                   deleted,
                   configuration,
                   regex_pattern as RegexPattern,
                   is_machine_name,
                   type
                FROM
                    public.apikeys
                WHERE
                    lower(key) = @key";

        private readonly string _connectionString;

        private readonly Dictionary<string, decimal[]> _exclusions = new() {
            {"bryce canyon", new[] {397995.510155659M, 4170226.5544169028M}},
            {"alta", new[] {446372M, 4493558M}},
            {"big water", new[] {440947.64679022622M, 4103827.0225703362M}},
            {"boulder", new[] {462787.3238216745M, 4195803.554941956M}}
        };

        public PostgreApiKeyRepository(IOptions<DatabaseConfiguration> dbOptions) {
            _connectionString = dbOptions.Value.ConnectionString;
        }

        public async Task<ApiKey> GetKey(string key) {
            key = key.ToLowerInvariant();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var items = await conn.QueryAsync<ApiKey>(ApiKeyByKey, new { key });

            return items.FirstOrDefault();
        }

        public async Task<IEnumerable<PlaceGridLink>> GetPlaceNames() {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            return await
                conn.QueryAsync<PlaceGridLink>("SELECT place, address_system as grid, weight from public.place_names");
        }

        public async Task<IEnumerable<ZipGridLink>> GetZipCodes() {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            return await
                conn.QueryAsync<ZipGridLink>("SELECT zip, address_system as grid, weight from public.zip_codes");
        }

        public async Task<IEnumerable<UspsDeliveryPointLink>> GetDeliveryPoints() {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            return await
                conn.QueryAsync<UspsDeliveryPointLink>("SELECT zip, address_system as grid, place, x, y from public.delivery_points");
        }

        public async Task<IDictionary<int, PoBoxAddress>> GetPoBoxes() {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var pos = await conn.QueryAsync<PoBoxAddress>("SELECT zip, x, y from public.po_boxes");

            return pos.ToDictionary(x => x.Zip, y => y);
        }

        public async Task<IEnumerable<PoBoxAddressCorrection>> GetCorrections() {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var corrections = await conn.QueryAsync("SELECT zip, zip_9 as zip9, place from public.zip_corrections");

            return corrections.Where(x => _exclusions.ContainsKey(x.place.ToLower()))
                              .Select(x => new PoBoxAddressCorrection(x.zip, x.zip9, _exclusions[x.place][0],
                                                                      _exclusions[x.place][1]));
        }
    }
}
