using System;
using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding
{
    public class AddressSystemFromCity
    {
        public class Command : IRequest<IEnumerable<GridLinkable>>
        {
            internal readonly string CityKey;

            public Command(string cityKey)
            {
                CityKey = cityKey;
            }
        }

        public class Handler : RequestHandler<Command, IEnumerable<GridLinkable>>
        {
            private readonly Dictionary<string, List<GridLinkable>> placeGrids;

            public Handler(ILookupCache driveCache)
            {
                placeGrids = driveCache.PlaceGrids;
            }

            protected override IEnumerable<GridLinkable> Handle(Command request)
            {
                Log.Debug("Getting address system from {city}", request.CityKey);

                if (string.IsNullOrEmpty(request.CityKey))
                {
                    return Enumerable.Empty<GridLinkable>();
                }

                placeGrids.TryGetValue(request.CityKey, out List<GridLinkable> gridLinkables);

                var result = gridLinkables ?? new List<GridLinkable>();

                Log.Debug("Found {systems}", result);

                return result;
            }
        }
    }
}
