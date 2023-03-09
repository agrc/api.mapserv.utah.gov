using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class AddressSystemFromZipCode {
        public class Computation : IComputation<IReadOnlyCollection<GridLinkable>> {
            internal readonly string Zip;

            public Computation(int? zip) {
                if (zip.HasValue) {
                    Zip = zip.ToString();
                }
            }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
            private readonly ILogger _log;
            private readonly ICacheRepository _memoryCache;

            public Handler(ICacheRepository cache, ILogger log) {
                _log = log?.ForContext<AddressSystemFromZipCode>();
                _memoryCache = cache;
            }

            public async Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
                _log.Debug("Getting address system from {city}", request.Zip);

                if (string.IsNullOrEmpty(request.Zip)) {
                    return Array.Empty<GridLinkable>();
                }

                var result = await _memoryCache.FindGridsForZipCodeAsync(request.Zip);

                _log.Debug("Found {systems}", result);

                return result;
            }
        }
    }
}
