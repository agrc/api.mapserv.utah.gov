using System;
using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding
{
    public class AddressSystemFromZipCode
    {
        public class Command : IRequest<IEnumerable<GridLinkable>>
        {
            public readonly string Zip;

            public Command(int? zip)
            {
                if (zip.HasValue)
                {
                    Zip = zip.ToString();
                }
            }
        }

        public class Handler : RequestHandler<Command, IEnumerable<GridLinkable>> {
            private readonly ILookupCache _driveCache;

            public Handler(ILookupCache driveCache)
            {
                _driveCache = driveCache;
            }

            protected override IEnumerable<GridLinkable> Handle(Command request)
            {
                Log.Debug("Getting address system from {city}", request.Zip);

                if (string.IsNullOrEmpty(request.Zip))
                {
                    return Enumerable.Empty<GridLinkable>();
                }

                _driveCache.ZipCodesGrids.TryGetValue(request.Zip, out List<GridLinkable> gridLinkables);

                var result = gridLinkables ?? new List<GridLinkable>();
                var grid = string.Join(",", result);

                Log.Debug("Found {systems}", result);

                return result;
            }
        }
    }
}
