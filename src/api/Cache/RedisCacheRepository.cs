using ugrc.api.Models.Linkables;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace ugrc.api.Cache;
public class RedisCacheRepository : ICacheRepository {
    private readonly IDatabase _db;
    private readonly MemoryCache _placeNameCache;
    private readonly MemoryCache _zipCodeCache;

    public RedisCacheRepository(Lazy<IConnectionMultiplexer> redis) {
        _db = redis.Value.GetDatabase();
        _placeNameCache = new MemoryCache(new MemoryCacheOptions {
            SizeLimit = 1000,
        });
        _zipCodeCache = new MemoryCache(new MemoryCacheOptions {
            SizeLimit = 600,
        });
    }

    /// <summary>
    /// Looks for the address system names for the given place name
    /// On the first lookup, the place name is looked up in the database and the results are cached
    /// On subsequent lookups, the place name is looked up in the found and not found cache
    /// </summary>
    /// <param name="placeName">the alias of a city or place</param>
    /// <returns>A GridLinkable with the grids</returns>
    public async Task<IReadOnlyCollection<GridLinkable>> FindGridsForPlaceAsync(string placeName) {
        // if the place name is null or empty, return nothing
        if (string.IsNullOrEmpty(placeName)) {
            return [];
        }

        placeName = placeName.ToLowerInvariant().Trim();

        // if the place name result is in memory already return it
        if (_placeNameCache.TryGetValue(placeName, out var places)) {
            await _db.StringIncrementAsync($"analytics:grid-hit:{placeName.ToLowerInvariant()}", 1, CommandFlags.FireAndForget);

            return places as IReadOnlyCollection<GridLinkable> ?? [];
        }

        // the item hasn't been looked up recently so look it up in the cache
        var place = await _db.StringGetAsync(placeName.ToLowerInvariant());

        // if the place name is not found, add it to the not found cache but check if the cache is full
        if (!place.IsNullOrEmpty) {
            var placeGridLinks = new List<PlaceGridLink>();

            foreach (var meta in place.ToString().Split(';')) {
                var parts = meta.Split(',');
                placeGridLinks.Add(new PlaceGridLink(placeName, parts[0], Convert.ToInt32(parts[1])));
            }

            _placeNameCache.Set(placeName, placeGridLinks, new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetSize(1));

            await _db.StringIncrementAsync($"analytics:grid-hit:{placeName.ToLowerInvariant()}", 1, CommandFlags.FireAndForget);

            return placeGridLinks;
        }

        // check to see if the cache is full
        var count = await _db.StringGetAsync("places");

        // if the cache has a value then the place isn't in our list
        if (count != RedisValue.Null) {
            await _db.StringIncrementAsync($"analytics:grid-miss:{placeName.ToLowerInvariant()}", 1, CommandFlags.FireAndForget);

            return [];
        }

        // TODO! hydrate the cache from BigQuery

        return [];
    }

    public async Task<IReadOnlyCollection<GridLinkable>> FindGridsForZipCodeAsync(string zipCode) {
        // if the place name is null or empty, return nothing
        if (string.IsNullOrEmpty(zipCode)) {
            return [];
        }

        zipCode = zipCode.ToLowerInvariant().Trim();

        // if the place name result is in memory already return it
        if (_zipCodeCache.TryGetValue(zipCode, out var places)) {
            await _db.StringIncrementAsync($"analytics:zip-hit:{zipCode.ToLowerInvariant()}", 1, CommandFlags.FireAndForget);

            return places as IReadOnlyCollection<GridLinkable> ?? [];
        }

        // the item hasn't been looked up recently so look it up in the cache
        var zip = await _db.StringGetAsync(zipCode.ToLowerInvariant());

        // if the place name is not found, add it to the not found cache but check if the cache is full
        if (!zip.IsNullOrEmpty) {
            var zipGridLinks = new List<ZipGridLink>();

            foreach (var meta in zip.ToString().Split(';')) {
                var parts = meta.Split(',');
                zipGridLinks.Add(new ZipGridLink(Convert.ToInt32(zipCode), parts[0], Convert.ToInt32(parts[1])));
            }

            _zipCodeCache.Set(zipCode, zipGridLinks, new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetSize(1));

            return zipGridLinks;
        }

        // check to see if the cache is full
        var count = await _db.StringGetAsync("zips");

        // if the cache has a value then the place isn't in our list
        if (count != RedisValue.Null) {
            await _db.StringIncrementAsync($"analytics:grid-miss:{zipCode.ToLowerInvariant()}", 1, CommandFlags.FireAndForget);

            return [];
        }

        // TODO! hydrate the cache from BigQuery

        return [];
    }
}
