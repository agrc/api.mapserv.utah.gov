using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class AddressSystemFromPlace {
        public class Computation : IComputation<IReadOnlyCollection<GridLinkable>> {
            public readonly string CityKey;

            public Computation(string cityKey) {
                CityKey = cityKey.ToLowerInvariant();
            }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
            private readonly ILogger _log;
            private readonly ICacheRepository _memoryCache;

            public Handler(ICacheRepository cache, ILogger log) {
                _log = log?.ForContext<AddressSystemFromPlace>();
                _memoryCache = cache;
            }

            public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
                _log.Debug("getting address system from {city}", request.CityKey);

                if (string.IsNullOrEmpty(request.CityKey)) {
                    return Array.Empty<GridLinkable>();
                }

                var result = await _memoryCache.FindGridsForPlaceAsync(request.CityKey);

                _log.Debug("found {systems}", result);

                return result;
            }
        }
    }
}
