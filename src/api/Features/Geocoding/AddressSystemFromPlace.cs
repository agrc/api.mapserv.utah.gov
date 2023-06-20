using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Features.Geocoding;
public class AddressSystemFromPlace {
    public class Computation(string cityKey) : IComputation<IReadOnlyCollection<GridLinkable>> {
        public readonly string _cityKey = cityKey.ToLowerInvariant();
    }

    public class Handler(ICacheRepository cache, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
        private readonly ILogger? _log = log?.ForContext<AddressSystemFromPlace>();
        private readonly ICacheRepository _memoryCache = cache;

        public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
            _log?.Debug("getting address system from city {city}", request._cityKey);

            if (string.IsNullOrEmpty(request._cityKey)) {
                return Array.Empty<GridLinkable>();
            }

            var result = await _memoryCache.FindGridsForPlaceAsync(request._cityKey);

            _log?.Debug("found {systems}", result);

            return result;
        }
    }
}
