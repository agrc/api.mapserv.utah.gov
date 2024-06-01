using ugrc.api.Cache;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Features.Geocoding;
public class AddressSystemFromPlace {
    public class Computation(string cityKey) : IComputation<IReadOnlyCollection<GridLinkable>> {
        public readonly string _cityKey = cityKey.ToLowerInvariant();
    }

    public class Handler(ICacheRepository cache, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
        private readonly ILogger? _log = log?.ForContext<AddressSystemFromPlace>();
        private readonly ICacheRepository _memoryCache = cache;

        public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
            _log?.Debug("Getting address system from city {city}", request._cityKey);

            if (string.IsNullOrEmpty(request._cityKey)) {
                return [];
            }

            var result = await _memoryCache.FindGridsForPlaceAsync(request._cityKey);

            if (result.Count == 0) {
                _log?.ForContext("place", request._cityKey)
                    .Information("Analytics:no-place");
            } else {
                _log?.ForContext("place", request._cityKey)
                .ForContext("grids", result)
                .Information("Analytics:place");
            }

            return result;
        }
    }
}
