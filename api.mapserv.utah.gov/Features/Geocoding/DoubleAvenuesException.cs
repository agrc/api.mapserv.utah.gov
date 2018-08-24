using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class DoubleAvenuesException {
        public class DoubleAvenueExceptionPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                where TRequest : ZoneParsing.Command
                where TResponse : GeocodeAddress {
            private readonly Regex _ordinal;

            public DoubleAvenueExceptionPipeline(IRegexCache cache) {
                _ordinal = cache.Get("avesOrdinal");
            }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
                var response = await next();

                if (response.PrefixDirection != Direction.None ||
                    response.StreetType != StreetType.Avenue || !IsOrdinal(response.StreetName)) {
                    Log.Debug("Only avenue addresses with no prefix are affected. skipping {address}", response);

                    return response;
                }

                Log.Debug("Possible double avenues exception {address}, {city}", response, request.InputZone);

                // it's in the problem area in midvale
                const int midvale = 84047;
                if (!string.IsNullOrEmpty(request.InputZone) && request.InputZone.ToLowerInvariant().Contains("midvale") ||
                    response.Zip5.HasValue && response.Zip5.Value == midvale) {
                    Log.Information("Midvale avenues exception, updating {response} to include West",
                                    response);

                    response.PrefixDirection = Direction.West;

                    return response;
                }

                // update the slc avenues to have an east
                if (response.AddressGrids.Select(x => x.Grid.ToLowerInvariant()).Contains("salt lake city")) {
                    Log.Information("SLC avenues exception, updating {response} to include East",
                                    response);

                    response.PrefixDirection = Direction.East;

                    return response;
                }

                Log.Debug("Not a double avenues exception {address}, {zone}", response, request.InputZone);

                return response;
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
