using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Features.Geocoding;
public class AddressSystemFromZipCode {
    public class Computation : IComputation<IReadOnlyCollection<GridLinkable>> {
        internal readonly string Zip = string.Empty;

        public Computation(int? zip) {
            if (zip.HasValue) {
                Zip = zip.ToString() ?? string.Empty;
            }
        }
    }

    public class Handler : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
        private readonly ILogger? _log;
        private readonly ICacheRepository _memoryCache;

        public Handler(ICacheRepository cache, ILogger log) {
            _log = log?.ForContext<AddressSystemFromZipCode>();
            _memoryCache = cache;
        }

        public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
            _log?.Debug("getting address system from zip {zip}", request.Zip);

            if (string.IsNullOrEmpty(request.Zip)) {
                return Array.Empty<GridLinkable>();
            }

            var result = await _memoryCache.FindGridsForZipCodeAsync(request.Zip);

            _log?.Debug("found {systems}", result);

            return result;
        }
    }
}
