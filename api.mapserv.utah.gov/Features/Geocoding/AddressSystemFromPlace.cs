using System;
using System.Collections.Generic;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class AddressSystemFromPlace {
        public class Command : IRequest<IReadOnlyCollection<GridLinkable>> {
            public readonly string CityKey;

            public Command(string cityKey) {
                CityKey = cityKey;
            }
        }

        public class Handler : RequestHandler<Command, IReadOnlyCollection<GridLinkable>> {
            private readonly ILogger _log;
            private readonly IDictionary<string, List<GridLinkable>> _placeGrids;

            public Handler(ILookupCache driveCache, ILogger log) {
                _log = log;
                _placeGrids = driveCache.PlaceGrids;
            }

            protected override IReadOnlyCollection<GridLinkable> Handle(Command request) {
                _log.Debug("Getting address system from {city}", request.CityKey);

                if (string.IsNullOrEmpty(request.CityKey)) {
                    return Array.Empty<GridLinkable>();
                }

                _placeGrids.TryGetValue(request.CityKey, out var gridLinkables);

                var result = gridLinkables ?? new List<GridLinkable>();

                _log.Debug("Found {systems}", result);

                return result;
            }
        }
    }
}
