using Google.Apis.Auth.OAuth2.Responses;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Services;
public class CacheHostedService : BackgroundService {
    private BigQueryClient? _client;
    private BigQueryTable? _table;
    private readonly IDatabase _db;
    private readonly ILogger? _log;

    public CacheHostedService(Lazy<IConnectionMultiplexer> redis, ILogger log) {
        _log = log?.ForContext<CacheHostedService>();
        _db = redis.Value.GetDatabase();
        TryGetBqTable(out _table);
    }

    private bool TryGetBqTable(out BigQueryTable? table) {
        table = null;

        _log?.Debug("Creating big query client");

        try {
            _client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GCLOUD_PROJECT") ?? "ut-dts-agrc-web-api-dev");
            table = _client.GetTable("address_grid_mapping_cache", "address_system_mapping");
        } catch (TokenResponseException ex) {
            _log?.Debug("Unable to connect to BigQuery. Cache is unavailable. " + ex.Message);

            return false;
        } catch (Exception ex) {
            _log?.Warning(ex, "Unable to connect to BigQuery. Cache is unavailable.");

            return false;
        }

        return true;
    }

    protected override async Task ExecuteAsync(CancellationToken token) {
        _log?.Debug("Checking redis cache for keys");

        while (_table is null) {
            if (!TryGetBqTable(out _table)) {
                _log?.Debug("Polling to create client bigquery client");

                await Task.Delay(5000, token);
                break;
            }
        }

        try {
            _log?.Warning("Rebuilding cache from bigquery.");

            await HydrateCacheFromBigQueryAsync(_db, token);
        } catch (Exception ex) {
            _log?
                .ForContext("db", _db)
                .ForContext("client", _client)
                .Warning("Trouble connecting to redis or rebuilding cache from bigquery.", ex);
        }
    }

    private async Task HydrateCacheFromBigQueryAsync(IDatabase db, CancellationToken token) {
        var places = new Dictionary<string, List<GridLinkable>>();
        var zips = new Dictionary<string, List<GridLinkable>>();

        if (_table is null || _client is null) {
            _log?
                .ForContext("table", _table)
                .ForContext("client", _client)
                .Warning("Error querying BigQuery. _client or table is null");

            return;
        }

        try {
            var addressSystemMapping = await _client.ExecuteQueryAsync(
                $"SELECT Zone, Address_System, Weight, Type FROM {_table} ORDER BY Zone, Weight", parameters: null, cancellationToken: token);

            foreach (var row in addressSystemMapping) {
                var addressSystem = row["Address_System"] as string ?? string.Empty;
                var weight = Convert.ToInt32(row["Weight"]);
                var type = row["Type"] as string ?? string.Empty;

                if (type == "place") {
                    var zone = row["Zone"] as string ?? string.Empty;
                    var item = new PlaceGridLink(zone.ToLowerInvariant(), addressSystem, weight);

                    if (places.TryGetValue(item.Key, out var value)) {
                        value.Add(item);

                        continue;
                    }

                    places.Add(item.Key, [item]);
                } else if (type == "zip") {
                    var success = int.TryParse(new ReadOnlySpan<char>((row["Zone"] as string ?? string.Empty).ToCharArray()), NumberStyles.Integer, CultureInfo.InvariantCulture, out var zone);
                    if (!success) {
                        _log?
                            .ForContext("row", row)
                            .Warning("invalid zip code in BigQuery");
                        continue;
                    }
                    var item = new ZipGridLink(zone, addressSystem, weight);

                    if (zips.TryGetValue(item.Key, out var value)) {
                        value.Add(item);

                        continue;
                    }

                    zips.Add(item.Key, [item]);
                }
            }
        } catch (Exception ex) {
            Console.WriteLine("Error querying BigQuery: " + ex.Message);
            _log?.Error(ex, "Error querying BigQuery");
        }

        foreach (var (key, value) in places) {
            await db.StringSetAsync($"map/place/{key}", string.Join(';', value));
        }

        foreach (var (key, value) in zips) {
            await db.StringSetAsync($"map/zip/{key}", string.Join(';', value));
        }

        if (places.Count == 0 || zips.Count == 0) {
            Console.WriteLine("Unable to hydrate Redis from BigQuery");
            throw new Exception("Unable to hydrate Redis from BigQuery");
        }

        Console.WriteLine("Setting key values places: " + places.Count + " zips: " + zips.Count);
        await db.StringSetAsync("map/places", DateTime.Now.ToString());
        await db.StringSetAsync("map/zips", DateTime.Now.ToString());
    }
}
