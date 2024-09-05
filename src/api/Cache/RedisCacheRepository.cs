using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using ugrc.api.Models.Linkables;

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
        var path = $"map/place/{placeName}";

        // if the place name result is in memory already return it
        if (_placeNameCache.TryGetValue(path, out var places)) {
            return places as IReadOnlyCollection<GridLinkable> ?? [];
        }

        // the item hasn't been looked up recently so look it up in the cache
        var place = await _db.StringGetAsync(path);

        // if the place name is not found, add it to the not found cache but check if the cache is full
        if (!place.IsNullOrEmpty) {
            var placeGridLinks = new List<PlaceGridLink>();

            foreach (var meta in place.ToString().Split(';')) {
                var parts = meta.Split(',');
                placeGridLinks.Add(new PlaceGridLink(placeName, parts[0], Convert.ToInt32(parts[1])));
            }

            _placeNameCache.Set(path, placeGridLinks, new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetSize(1));

            return placeGridLinks;
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
        var path = $"map/zip/{zipCode}";

        // if the place name result is in memory already return it
        if (_zipCodeCache.TryGetValue(path, out var places)) {
            return places as IReadOnlyCollection<GridLinkable> ?? [];
        }

        // the item hasn't been looked up recently so look it up in the cache
        var zip = await _db.StringGetAsync(zipCode);

        // if the place name is not found, add it to the not found cache but check if the cache is full
        if (!zip.IsNullOrEmpty) {
            var zipGridLinks = new List<ZipGridLink>();

            foreach (var meta in zip.ToString().Split(';')) {
                var parts = meta.Split(',');
                zipGridLinks.Add(new ZipGridLink(Convert.ToInt32(zipCode), parts[0], Convert.ToInt32(parts[1])));
            }

            _zipCodeCache.Set(path, zipGridLinks, new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetSize(1));

            return zipGridLinks;
        }

        // TODO! hydrate the cache from BigQuery

        return [];
    }
}
