using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models.Linkables;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Hosting;
using Serilog;
using StackExchange.Redis;

namespace AGRC.api.Services {
    public class CacheHostedService : IHostedService {
        private readonly BigQueryClient _client;
        private readonly BigQueryTable _table;
        private readonly IDatabase _db;
        private readonly ILogger _log;

        public CacheHostedService(ConnectionMultiplexer redis, ILogger log) {
            _client = BigQueryClient.Create("ut-dts-agrc-web-api-dev");
            _table = _client.GetTable("address_grid_mapping_cache", "address_system_mapping");
            _db = redis.GetDatabase();
            _log = log?.ForContext<CacheHostedService>();
        }
        public async Task StartAsync(CancellationToken token) {
            try {
                var keys = await _db.KeyExistsAsync(new RedisKey[] { "places", "zips" });
                if (keys != 2) {
                    _log.Warning("Cache is missing keys. Rebuilding cache from BigQuery.");

                    await HydrateCacheFromBigQueryAsync(_db, token);
                }
            } catch (Exception) {
                // TODO! log error
            }
        }

        private async Task HydrateCacheFromBigQueryAsync(IDatabase db, CancellationToken token) {
            var places = new Dictionary<string, List<GridLinkable>>();
            var zips = new Dictionary<string, List<GridLinkable>>();

            try {
                var addressSystemMapping = await _client.ExecuteQueryAsync(
                    $"SELECT Zone, Address_System, Weight, Type FROM {_table} ORDER BY Zone, Weight", parameters: null, cancellationToken: token);

                foreach (var row in addressSystemMapping) {
                    var addressSystem = row["Address_System"] as string;
                    var weight = Convert.ToInt32(row["Weight"]);
                    var type = row["Type"] as string;

                    if (type == "place") {
                        var zone = row["Zone"] as string;
                        var item = new PlaceGridLink(zone, addressSystem, weight);

                        if (places.TryGetValue(item.Key, out var value)) {
                            value.Add(item);

                            continue;
                        }

                        places.Add(item.Key, new List<GridLinkable> { item });
                    } else if (type == "zip") {
                        var zone = Convert.ToInt32(row["Zone"]);
                        var item = new ZipGridLink(zone, addressSystem, weight);

                        if (zips.TryGetValue(item.Key, out var value)) {
                            value.Add(item);

                            continue;
                        }

                        zips.Add(item.Key, new List<GridLinkable> { item });
                    }
                }
            } catch (Exception ex) {
                _log.Error(ex, "Error querying BigQuery");
            }

            foreach (var (key, value) in places) {
                await db.StringSetAsync(key, string.Join(';', value));
            }

            foreach (var (key, value) in zips) {
                await db.StringSetAsync(key, string.Join(';', value));
            }

            if (places.Count == 0 || zips.Count == 0) {
                throw new Exception("Unable to hydrate Redis from BigQuery");
            }

            await db.StringSetAsync("places", places.Count);
            await db.StringSetAsync("zips", zips.Count);
        }

        public Task StopAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}
