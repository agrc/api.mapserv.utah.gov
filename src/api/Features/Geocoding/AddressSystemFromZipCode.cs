using ugrc.api.Cache;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Features.Geocoding;
public class AddressSystemFromZipCode {
    public class Computation : IComputation<IReadOnlyCollection<GridLinkable>> {
        public readonly string _zip = string.Empty;

        public Computation(int? zip) {
            if (zip.HasValue) {
                _zip = zip.ToString() ?? string.Empty;
            }
        }
    }

    public class Handler(ICacheRepository cache, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
        private readonly ILogger? _log = log?.ForContext<AddressSystemFromZipCode>();
        private readonly ICacheRepository _memoryCache = cache;

        public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
            _log?.Debug("Getting address system from zip {zip}", request._zip);

            if (string.IsNullOrEmpty(request._zip)) {
                return [];
            }

            var result = await _memoryCache.FindGridsForZipCodeAsync(request._zip);

            if (result.Count == 0) {
                _log?.ForContext("zone", request._zip)
                    .Information("Analytics:no-zone");
            } else {
                _log?.ForContext("zone", request._zip)
                    .ForContext("grids", result)
                    .Information("Analytics:zone");
            }

            return result;
        }
    }
}
