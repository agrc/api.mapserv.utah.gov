using Google.Apis.Auth.OAuth2.Responses;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Services;

public class CacheHostedService(IMemoryCache cache, ILogger log) : BackgroundService {
    private BigQueryClient? _client;
    private BigQueryTable? _table;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger? _log = log?.ForContext<CacheHostedService>();

    private bool InitializeBigQueryAssets(out BigQueryTable? table) {
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
        await UpdateCacheAsync(token); // Initial update on startup

        while (!token.IsCancellationRequested) {
            var now = DateTime.UtcNow;
            var nextSunday = now.AddDays(DayOfWeek.Sunday - now.DayOfWeek);
            if (nextSunday < now) // If today is Sunday, schedule for next Sunday
            {
                nextSunday = nextSunday.AddDays(7);
            }

            var delay = nextSunday - now;

            _log?.Information($"Next cache update scheduled for {nextSunday:G} (in {delay}).");

            await Task.Delay(delay, token);

            await UpdateCacheAsync(token);
        }
    }

    private async Task UpdateCacheAsync(CancellationToken token) {
        var places = new Dictionary<string, List<GridLinkable>>();
        var zips = new Dictionary<string, List<GridLinkable>>();

        if (_client is null || _table is null) {
            var tries = 4;
            var ok = InitializeBigQueryAssets(out _table);

            while (!ok && tries-- > 0) {
                await Task.Delay(500, token);

                ok = InitializeBigQueryAssets(out _table);
            }

            if (!ok) {
                ArgumentNullException.ThrowIfNull(_client);
            }
        }

        _log?.Information("Updating BigQuery cache...");

        var addressSystemMapping = await QueryBigQueryDataAsync(token);

        foreach (var row in addressSystemMapping) {
            var type = row["Type"] as string ?? string.Empty;
            var addressSystem = row["Address_System"] as string ?? string.Empty;
            var weight = Convert.ToInt32(row["Weight"]);
            var zone = row["Zone"] as string ?? string.Empty;

            switch (type) {
                case "place":
                    AddPlaceMapping(places, zone, addressSystem, weight);
                    break;
                case "zip":
                    AddZipMapping(zips, zone, addressSystem, weight, _log);
                    break;
            }
        }

        UpdateCache(places, zips);

        _log?.Information("BigQuery cache updated successfully.");
    }

    private async Task<BigQueryResults> QueryBigQueryDataAsync(CancellationToken token) {
        ArgumentNullException.ThrowIfNull(_client);
        ArgumentNullException.ThrowIfNull(_table);

        return await _client.ExecuteQueryAsync(
        $"SELECT Zone, Address_System, Weight, Type FROM {_table} ORDER BY Zone, Weight",
        parameters: null,
        cancellationToken: token);
    }

    private static void AddPlaceMapping(Dictionary<string, List<GridLinkable>> places, string zone, string addressSystem, int weight) {
        var item = new PlaceGridLink(zone.ToLowerInvariant(), addressSystem, weight);

        if (!places.TryGetValue(item.Key, out var value)) {
            places[item.Key] = [item];

            return;
        }

        value.Add(item);
    }

    private static void AddZipMapping(Dictionary<string, List<GridLinkable>> zips, string zone, string addressSystem, int weight, ILogger? log) {
        if (!int.TryParse(zone, NumberStyles.Integer, CultureInfo.InvariantCulture, out var zipCode)) {
            log?.Warning("Invalid zip code {Zone} in BigQuery", zone);

            return;
        }

        var item = new ZipGridLink(zipCode, addressSystem, weight);

        if (!zips.TryGetValue(item.Key, out var value)) {
            zips[item.Key] = [item];

            return;
        }

        value.Add(item);
    }

    private void UpdateCache(Dictionary<string, List<GridLinkable>> places, Dictionary<string, List<GridLinkable>> zips) {
        foreach (var (key, value) in places) {
            _cache.Set($"mapping/place/{key}", value);
        }
        _cache.Set("mapping/places", places.Keys.ToList());

        foreach (var (key, value) in zips) {
            _cache.Set($"mapping/zip/{key}", value);
        }
    }
}
