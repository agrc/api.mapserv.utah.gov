using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models.Linkables;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Hosting;
using Serilog;
using StackExchange.Redis;

namespace AGRC.api.Services {
    public class CacheHostedService : BackgroundService {
        private BigQueryClient _client;
        private BigQueryTable _table;
        private readonly IDatabase _db;
        private readonly ILogger _log;

        public CacheHostedService(Lazy<IConnectionMultiplexer> redis, ILogger log) {
            _log = log?.ForContext<CacheHostedService>();
            _db = redis.Value.GetDatabase();
            TryGetBqTable(out _table);
        }

        private bool TryGetBqTable(out BigQueryTable table) {
            table = null;

            _log.Debug("creating big query client");

            try {
                _client = BigQueryClient.Create("ut-dts-agrc-web-api-dev");
                table = _client.GetTable("address_grid_mapping_cache", "address_system_mapping");
            } catch (TokenResponseException ex) {
                _log.Debug("unable to connect to BigQuery. Cache is unavailable. " + ex.Message);

                return false;
            } catch (Exception ex) {
                _log.Warning(ex, "unable to connect to BigQuery. Cache is unavailable.");

                return false;
            }

            return true;
        }

        protected override async Task ExecuteAsync(CancellationToken token) {
            await Task.Delay(5000, token);

            _log.Debug("checking redis cache for keys");

            while (_table is null) {
                if (!TryGetBqTable(out _table)) {
                    _log.Debug("polling to create client bigquery client");

                    await Task.Delay(5000, token);
                    break;
                }
            }

            try {
                var keys = await _db.KeyExistsAsync(new RedisKey[] { "places", "zips" });

                if (keys != 2) {
                    _log.Warning("redis cache is missing keys. rebuilding cache from bigquery.");

                    await HydrateCacheFromBigQueryAsync(_db, token);
                } else {
                    _log.Debug("redis cache is ready.");
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
                Console.WriteLine("Error querying BigQuery: " + ex.Message);
                _log.Error(ex, "Error querying BigQuery");
            }

            foreach (var (key, value) in places) {
                await db.StringSetAsync(key, string.Join(';', value));
            }

            foreach (var (key, value) in zips) {
                await db.StringSetAsync(key, string.Join(';', value));
            }

            if (places.Count == 0 || zips.Count == 0) {
                Console.WriteLine("Unable to hydrate Redis from BigQuery");
                throw new Exception("Unable to hydrate Redis from BigQuery");
            }

            Console.WriteLine("setting key values places: " + places.Count + " zips: " + zips.Count);
            await db.StringSetAsync("places", places.Count);
            await db.StringSetAsync("zips", zips.Count);
        }
    }
}
