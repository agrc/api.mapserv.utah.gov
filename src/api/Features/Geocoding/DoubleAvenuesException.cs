using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;

namespace AGRC.api.Features.Geocoding;
public class DoubleAvenuesException {
    public class Decorator : IComputationHandler<ZoneParsing.Computation, Address> {
        private readonly IComputationHandler<ZoneParsing.Computation, Address> _decorated;
        private readonly ILogger? _log;
        private readonly Regex _ordinal;

        public Decorator(IComputationHandler<ZoneParsing.Computation, Address> decorated, IRegexCache cache, ILogger log) {
            _log = log?.ForContext<DoubleAvenuesException>();
            _ordinal = cache.Get("avesOrdinal");
            _decorated = decorated;
        }
        public async Task<Address> Handle(ZoneParsing.Computation computation, CancellationToken cancellationToken) {
            var response = await _decorated.Handle(computation, cancellationToken);

            if (response.PrefixDirection != Direction.None ||
                response.StreetType != StreetType.Avenue || !IsOrdinal(response.StreetName)) {
                _log?.Debug("no candidate");

                return response;
            }

            _log?.ForContext("street", response.StandardizedAddress)
                .ForContext("zone", computation.InputZone)
                .Debug("possible candidate");

            // it's in the problem area in midvale
            const int midvale = 84047;
            if ((!string.IsNullOrEmpty(computation.InputZone) &&
                computation.InputZone.Contains("midvale", StringComparison.InvariantCultureIgnoreCase)) ||
                (response.Zip5 == midvale)) {
                _log?.Information("midvale avenues match");

                return response.SetPrefixDirection(Direction.West);
            }

            // update the slc avenues to have an east
            if (response.AddressGrids.Select(x => x.Grid.ToLowerInvariant()).Contains("salt lake city")) {
                _log?.Information("slc avenues match");

                return response.SetPrefixDirection(Direction.East);
            }

            _log?.Debug("no match");

            return response;
        }

        private bool IsOrdinal(string streetName) {
            if (string.IsNullOrWhiteSpace(streetName)) {
                return false;
            }

            streetName = streetName.Replace(" ", string.Empty).Trim();

            return _ordinal.IsMatch(streetName);
        }
    }
}
