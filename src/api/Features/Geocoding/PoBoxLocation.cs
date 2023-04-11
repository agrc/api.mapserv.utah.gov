using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Features.Geocoding;
public class PoBoxLocation {
    public class Computation : IComputation<Candidate?>, IHasGeocodingOptions {
        internal readonly Address Address;

        public Computation(Address address, SingleGeocodeRequestOptionsContract options) {
            Address = address;
            Options = options;
        }

        public SingleGeocodeRequestOptionsContract Options { get; }
    }

    public class Handler : IComputationHandler<Computation, Candidate?> {
        private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions;
        private readonly ILogger? _log;
        private readonly IDictionary<int, PoBoxAddress> _poBoxes;
        private readonly IReadOnlyCollection<int> _zipExclusions;

        public Handler(IStaticCache cache, ILogger log) {
            _poBoxes = cache.PoBoxes;
            _exclusions = cache.PoBoxExclusions;
            _zipExclusions = cache.PoBoxZipCodesWithExclusions;
            _log = log?.ForContext<PoBoxLocation>();
        }

        public Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (!request.Address.Zip5.HasValue) {
                _log?.Debug("no candidate");

                return Task.FromResult<Candidate?>(null);
            }

            if (_poBoxes is null) {
                _log?.Warning("cache is empty");

                return Task.FromResult<Candidate?>(null);
            }

            if (!_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                _log?.ForContext("zip", request.Address.Zip5.Value)
                    .Debug("cache miss");

                return Task.FromResult<Candidate?>(null);
            }

            Candidate candidate;
            var key = (request.Address.Zip5.Value * 10000) + request.Address.PoBox;

            if (_zipExclusions.Any(x => x == request.Address.Zip5) &&
                _exclusions.TryGetValue(key, out var value)) {
                _log?.ForContext("post office exclusion", key)
                    .Information("match");

                var exclusion = value;
                candidate = new Candidate(
                     request.Address.StandardizedAddress,
                     request.Address.AddressGrids.FirstOrDefault()?.Grid ?? "unknown",
                     new Point(exclusion.X, exclusion.Y),
                     100,
                     "Post Office Point Exclusions",
                     0
                );
            } else if (_poBoxes.TryGetValue(request.Address.Zip5.Value, out var result)) {
                _log?.Information("match");

                candidate = new Candidate(
                     request.Address.StandardizedAddress,
                     request.Address.AddressGrids.FirstOrDefault()?.Grid ?? "unknown",
                     new Point(result.X, result.Y),
                     100,
                     "Post Office Point",
                     0
                );
            } else {
                return Task.FromResult<Candidate?>(null);
            }

            return Task.FromResult<Candidate?>(candidate);
        }
    }
}
