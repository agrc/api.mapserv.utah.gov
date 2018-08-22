using System.Linq;
using System.Text.RegularExpressions;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class DoubleAvenuesException {
        public class Command : IRequest<GeocodeAddress> {
            public readonly GeocodeAddress Address;
            internal readonly string City;

            public Command(GeocodeAddress address, string city) {
                Address = address;
                City = city ?? "";
                City = City.Trim();
            }
        }

        public class Handler : RequestHandler<Command, GeocodeAddress> {
            private readonly Regex _ordinal;

            public Handler(IRegexCache cache) {
                _ordinal = cache.Get("avesOrdinal");
            }

            protected override GeocodeAddress Handle(Command request) {
                // only avenue addresses with no prefix are affected
                if (request.Address.PrefixDirection != Direction.None ||
                    request.Address.StreetType != StreetType.Avenue || !IsOrdinal(request.Address.StreetName)) {
                    Log.Debug("Only avenue addresses with no prefix are affected. skipping {address}", request.Address);

                    return request.Address;
                }

                Log.Debug("Possible double avenues exception {address}, {city}", request.Address, request.City);

                // it's in the problem area in midvale
                const int midvale = 84047;
                if (!string.IsNullOrEmpty(request.City) && request.City.ToUpperInvariant().Contains("MIDVALE") ||
                    request.Address.Zip5.HasValue && request.Address.Zip5.Value == midvale) {
                    Log.Information("Midvale avenues exception, updating {request.address} to include West",
                                    request.Address);

                    request.Address.PrefixDirection = Direction.West;

                    return request.Address;
                }

                // update the slc avenues to have an east
                if (request.Address.AddressGrids.Select(x => x.Grid).Contains("SALT LAKE CITY")) {
                    Log.Information("SLC avenues exception, updating {request.address} to include East",
                                    request.Address);

                    request.Address.PrefixDirection = Direction.East;

                    return request.Address;
                }

                Log.Debug("Not a double avenues exception", request.Address, request.City);

                return request.Address;
            }

            private bool IsOrdinal(string streetname) {
                if (string.IsNullOrWhiteSpace(streetname)) {
                    return false;
                }

                streetname = streetname.Replace(" ", string.Empty).Trim();

                return _ordinal.IsMatch(streetname);
            }
        }
    }
}
