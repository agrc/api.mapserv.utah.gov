using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.SecretOptions;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace api.mapserv.utah.gov.Services
{
    public class PostgreApiKeyRepository : IApiKeyRepository, ICacheRepository
    {
        private readonly Dictionary<string, decimal[]> exclusions = new Dictionary<string, decimal[]>
            {
                {"bryce canyon", new[] {397995.510155659M, 4170226.5544169028M}},
                {"alta", new[] {446372M, 4493558M}},
                {"big water", new[] {440947.64679022622M, 4103827.0225703362M}},
                {"boulder", new[] {462787.3238216745M, 4195803.554941956M}}
            };
        private readonly string ConnectionString;
        private const string apiKeyByKey = @"SELECT key,
                   account_id,
                   whitelisted,
                   enabled,
                   deleted,
                   configuration,
                   regex_pattern,
                   is_machine_name
                FROM
                    public.apikeys
                WHERE
                    lower(key) = @key";

        public PostgreApiKeyRepository(IOptions<DatabaseConfiguration> dbOptions)
        {
            ConnectionString = dbOptions.Value.ConnectionString;
        }

        public async Task<ApiKey> GetKey(string key)
        {
            key = key.ToLowerInvariant();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var items = await conn.QueryAsync<ApiKey>(apiKeyByKey, new { key });

                return items.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<PlaceGridLink>> GetPlaceNames()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                return await conn.QueryAsync<PlaceGridLink>("SELECT place, address_system as grid, weight from public.place_names");
            }
        }

        public async Task<IEnumerable<ZipGridLink>> GetZipCodes()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                return await conn.QueryAsync<ZipGridLink>("SELECT zip, address_system as grid, weight from public.zip_codes");
            }
        }

        public async Task<IEnumerable<UspsDeliveryPointLink>> GetDeliveryPoints()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                return await conn.QueryAsync<UspsDeliveryPointLink>("SELECT zip, address_system as grid, place, x, y from public.delivery_points");
            }
        }

        public async Task<IDictionary<int, PoBoxAddress>> GetPoBoxes()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var pos = await conn.QueryAsync<PoBoxAddress>("SELECT zip, x, y from public.po_boxes");

                return pos.ToDictionary(x => x.Zip, y => y);
            }
        }

        public async Task<IEnumerable<PoBoxAddressCorrection>> GetCorrections()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var corrections = await conn.QueryAsync("SELECT zip, zip_9 as zip9, place from public.zip_corrections");

                return corrections.Where(x => exclusions.ContainsKey(x.place.ToLower()))
                           .Select(x => new PoBoxAddressCorrection(x.zip, x.zip9, exclusions[x.place][0], exclusions[x.place][1]));
            }
        }
    }
}
