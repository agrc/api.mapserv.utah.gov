using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models.Linkables;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class AddressSystemFromPlace {
        public class Computation : IComputation<IReadOnlyCollection<GridLinkable>> {
            public readonly string CityKey;

            public Computation(string cityKey) {
                CityKey = cityKey;
            }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<GridLinkable>> {
            private readonly ILogger _log;
            private readonly IDictionary<string, List<GridLinkable>> _placeGrids;

            public Handler(ILookupCache driveCache, ILogger log) {
                _log = log?.ForContext<AddressSystemFromPlace>();
                _placeGrids = driveCache.PlaceGrids;
            }

            public Task<IReadOnlyCollection<GridLinkable>> Handle(Computation request, CancellationToken cancellationToken) {
                _log.Debug("Getting address system from {city}", request.CityKey);

                if (string.IsNullOrEmpty(request.CityKey)) {
                    return Task.FromResult<IReadOnlyCollection<GridLinkable>>(Array.Empty<GridLinkable>());
                }

                _placeGrids.TryGetValue(request.CityKey, out var gridLinkables);

                var result = gridLinkables ?? new List<GridLinkable>();

                _log.Debug("Found {systems}", result);

                return Task.FromResult<IReadOnlyCollection<GridLinkable>>(result);
            }
        }
    }
}
