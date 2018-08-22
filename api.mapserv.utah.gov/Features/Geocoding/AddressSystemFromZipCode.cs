using System;
using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class AddressSystemFromZipCode {
        public class Command : IRequest<IReadOnlyCollection<GridLinkable>> {
            internal readonly string Zip;

            public Command(int? zip) {
                if (zip.HasValue) {
                    Zip = zip.ToString();
                }
            }
        }

        public class Handler : RequestHandler<Command, IReadOnlyCollection<GridLinkable>> {
            private readonly IDictionary<string, List<GridLinkable>> _zipCache;

            public Handler(ILookupCache driveCache) {
                _zipCache = driveCache.ZipCodesGrids;
            }

            protected override IReadOnlyCollection<GridLinkable> Handle(Command request) {
                Log.Debug("Getting address system from {city}", request.Zip);

                if (string.IsNullOrEmpty(request.Zip)) {
                    return Array.Empty<GridLinkable>();
                }

                _zipCache.TryGetValue(request.Zip, out var gridLinkables);

                var result = gridLinkables ?? new List<GridLinkable>();

                Log.Debug("Found {systems}", result);

                return result;
            }
        }
    }
}
