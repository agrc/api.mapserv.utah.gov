using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class DoubleAvenuesException {
        public class Decorator : IComputationHandler<ZoneParsing.Computation, AddressWithGrids> {
            private readonly IComputationHandler<ZoneParsing.Computation, AddressWithGrids> _decorated;
            private readonly ILogger _log;
            private readonly Regex _ordinal;

            public Decorator(IComputationHandler<ZoneParsing.Computation, AddressWithGrids> decorated, IRegexCache cache, ILogger log) {
                _log = log?.ForContext<DoubleAvenuesException>();
                _ordinal = cache.Get("avesOrdinal");
                _decorated = decorated;
            }
            public async Task<AddressWithGrids> Handle(ZoneParsing.Computation computation, CancellationToken cancellationToken) {
                var response = await _decorated.Handle(computation, cancellationToken);

                if (response.PrefixDirection != Direction.None ||
                    response.StreetType != StreetType.Avenue || !IsOrdinal(response.StreetName)) {
                    _log.Debug("not a candidate");

                    return response;
                }

                _log.ForContext("street", response.StandardizedAddress)
                    .ForContext("zone", computation.InputZone)
                    .Debug("possible candidate");

                // it's in the problem area in midvale
                const int midvale = 84047;
                if (!string.IsNullOrEmpty(computation.InputZone) &&
                    computation.InputZone.ToLowerInvariant().Contains("midvale") ||
                    response.Zip5.HasValue && response.Zip5.Value == midvale) {
                    _log.Information("midvale avenues match");

                    response.PrefixDirection = Direction.West;

                    return response;
                }

                // update the slc avenues to have an east
                if (response.AddressGrids.Select(x => x.Grid.ToLowerInvariant()).Contains("salt lake city")) {
                    _log.Information("slc avenues match");

                    response.PrefixDirection = Direction.East;

                    return response;
                }

                _log.Debug("no match");

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
