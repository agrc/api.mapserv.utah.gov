using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Features.Geocoding;
public class PoBoxLocation {
    public class Computation(Address address, SingleGeocodeRequestOptionsContract options) : IComputation<Candidate?>, IHasGeocodingOptions {
        public readonly Address _address = address;

        public SingleGeocodeRequestOptionsContract Options { get; } = options;
    }

    public class Handler(IStaticCache cache, ILogger log) : IComputationHandler<Computation, Candidate?> {
        private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions = cache.PoBoxExclusions;
        private readonly ILogger? _log = log?.ForContext<PoBoxLocation>();
        private readonly IDictionary<int, PoBoxAddress> _poBoxes = cache.PoBoxes;
        private readonly IReadOnlyCollection<int> _zipExclusions
         = cache.PoBoxZipCodesWithExclusions;

        public Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (!request._address.Zip5.HasValue) {
                _log?.Debug("no candidate");

                return Task.FromResult<Candidate?>(null);
            }

            if (_poBoxes is null) {
                _log?.Warning("cache is empty");

                return Task.FromResult<Candidate?>(null);
            }

            if (!_poBoxes.ContainsKey(request._address.Zip5.Value)) {
                _log?.ForContext("zip", request._address.Zip5.Value)
                    .Debug("cache miss");

                return Task.FromResult<Candidate?>(null);
            }

            Candidate candidate;
            var key = (request._address.Zip5.Value * 10000) + request._address.PoBox;

            if (_zipExclusions.Any(x => x == request._address.Zip5) &&
                _exclusions.TryGetValue(key, out var value)) {
                _log?.ForContext("post office exclusion", key)
                    .Information("match");

                var exclusion = value;
                candidate = new Candidate(
                     request._address.StandardizedAddress(),
                     request._address.AddressGrids.FirstOrDefault()?.Grid ?? "unknown",
                     new Point(exclusion.X, exclusion.Y),
                     100,
                     "Post Office Point Exclusions",
                     0
                );
            } else if (_poBoxes.TryGetValue(request._address.Zip5.Value, out var result)) {
                _log?.Information("match");

                candidate = new Candidate(
                     request._address.StandardizedAddress(),
                     request._address.AddressGrids.FirstOrDefault()?.Grid ?? "unknown",
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

public record PoBoxAddress(int Zip, double X, double Y);
public record PoBoxAddressCorrection(int ZipPlusFour, int Zip, double X, double Y) : PoBoxAddress(Zip, X, Y);
