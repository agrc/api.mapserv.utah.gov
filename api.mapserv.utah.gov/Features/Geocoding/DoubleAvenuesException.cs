using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding
{
    public class DoubleAvenuesException
    {
        public class Command : IRequest<GeocodeAddress>{
            internal readonly GeocodeAddress address;
            internal readonly string city;

            public Command(GeocodeAddress address, string city)
            {
                this.address = address;
                this.city = city ?? "";
                this.city = this.city.Trim();
            }
        }

        public class Handler : RequestHandler<Command, GeocodeAddress>
        {
            private readonly Regex _ordinal;

            public Handler(IRegexCache cache)
            {
                _ordinal = cache.Get("avesOrdinal");
            }

            protected override GeocodeAddress Handle(Command request)
            {
                // only avenue addresses with no prefix are affected
                if (request.address.PrefixDirection != Direction.None || request.address.StreetType != StreetType.Avenue || !IsOrdinal(request.address.StreetName))
                {
                    Log.Debug("Only avenue addresses with no prefix are affected. skipping {address}", request.address);

                    return request.address;
                }

                Log.Debug("Possible double avenues exception {address}, {city}", request.address, request.city);

                // it's in the problem area in midvale
                const int midvale = 84047;
                if (!string.IsNullOrEmpty(request.city) && request.city.ToUpperInvariant().Contains("MIDVALE") ||
                    request.address.Zip5.HasValue && request.address.Zip5.Value == midvale)
                {
                    Log.Information("Midvale avenues exception, updating {request.address} to include West", request.address);

                    request.address.PrefixDirection = Direction.West;

                    return request.address;
                }

                // update the slc avenues to have an east
                if (request.address.AddressGrids.Select(x => x.Grid).Contains("SALT LAKE CITY"))
                {
                    Log.Information("SLC avenues exception, updating {request.address} to include East", request.address);

                    request.address.PrefixDirection = Direction.East;

                    return request.address;
                }

                Log.Debug("Not a double avenues exception", request.address, request.city);

                return request.address;
            }

            private bool IsOrdinal(string streetname)
            {
                if (string.IsNullOrWhiteSpace(streetname))
                {
                    return false;
                }

                streetname = streetname.Replace(" ", string.Empty).Trim();

                return _ordinal.IsMatch(streetname);
            }
        }
    }
}
