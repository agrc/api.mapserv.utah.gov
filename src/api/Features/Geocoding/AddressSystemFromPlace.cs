using Fastenshtein;
using Microsoft.Extensions.Caching.Memory;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;
using ugrc.api.Services;
using ZiggyCreatures.Caching.Fusion;

namespace ugrc.api.Features.Geocoding;
public class AddressSystemFromPlace {
    public class Computation(string cityKey) : IComputation<IReadOnlyCollection<GridLinkable>> {
        public readonly string _cityKey = cityKey.ToLowerInvariant();
    }

    public class Handler(IMemoryCache cache, IFusionCacheProvider provider, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
        private readonly ILogger? _log = log?.ForContext<AddressSystemFromPlace>();
        private readonly IMemoryCache _memoryCache = cache;
        private readonly IFusionCache _fusionCache = provider.GetCache("places");

        public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken token) {
            _log?.Debug("Getting address system for city {city}", request._cityKey);

            if (string.IsNullOrEmpty(request._cityKey)) {
                return [];
            }

            // Check memory cache first
            if (IsPlaceNameMatch(request._cityKey, out var result)) {
                return result;
            }

            // Check fusion cache next
            var (success, fuzzyResult) = await IsFuzzyMatchAsync(request._cityKey, token);
            if (success) {
                return fuzzyResult;
            }

            // levenshtein everything and check again
            var (successful, newFuzzyResult) = await GetAndSetFuzzyMatchAsync(request._cityKey, token);
            if (successful) {
                return newFuzzyResult;
            }

            await LogMissAndSetCache(request._cityKey, "miss levenshtein", token);
            return [];
        }
        private bool IsPlaceNameMatch(string key, out List<GridLinkable> result) {
            result = _memoryCache.Get<List<GridLinkable>>($"mapping/place/{key}") ?? [];

            if (result.Count > 0) {
                LogCacheHit(key, "bigquery");

                return true;
            }

            return false;
        }
        private async Task<(bool success, List<GridLinkable> result)> IsFuzzyMatchAsync(string key, CancellationToken token) {
            var result = await _fusionCache.GetOrDefaultAsync<List<GridLinkable>>($"mapping/place/{key}", defaultValue: [], token: token);

            if (result!.Count > 0) {
                LogCacheHit(key, "fusion");

                return (true, result);
            }

            return (false, result);
        }
        private async Task<(bool success, List<GridLinkable> fuzzyResult)> GetAndSetFuzzyMatchAsync(string key, CancellationToken token) {
            // Try get all the keys to levenshtein
            var places = _memoryCache.Get<List<string>>("mapping/places");
            if (places is null || places.Count == 0) {
                // This shouldn't happen but it could?
                await LogMissAndSetCache(key, "empty mapping/places", token);

                return (false, []);
            }

            var closestMatch = FindClosestMatch(key, places);
            if (string.IsNullOrEmpty(closestMatch)) {
                await LogMissAndSetCache(key, "empty levenshtein", token);

                return (false, []);
            }

            var result = _memoryCache.Get<List<GridLinkable>>($"mapping/place/{closestMatch}");
            if (result?.Count > 0) {
                LogCacheHit(closestMatch, "levenshtein");
                await _fusionCache.SetAsync($"mapping/place/{key}", result, token: token);

                return (true, result);
            }

            return (false, []);
        }
        private static string? FindClosestMatch(string key, List<string> places) {
            var lev = new Levenshtein(key);
            var priorityQueue = new LowestDistance(1);

            places.ForEach(place => priorityQueue.Add(new(lev.DistanceFrom(place), place)));

            var closestMatch = priorityQueue.Get().FirstOrDefault(new Map(int.MaxValue, string.Empty));

            return closestMatch.Difference > 2 ? null : closestMatch.Zone;
        }
        private void LogCacheHit(string place, string cacheType)
        => _log?.ForContext("place", place)
                .ForContext("cache", cacheType)
                .Information("Analytics:place");
        private async Task LogMissAndSetCache(string place, string reason, CancellationToken token) {
            _log?.ForContext("place", place)
                .ForContext("reason", reason)
                .Information("Analytics:no-place");

            await _fusionCache.SetAsync<List<GridLinkable>>($"mapping/place/{place}", [], token: token);
        }
    }
}
