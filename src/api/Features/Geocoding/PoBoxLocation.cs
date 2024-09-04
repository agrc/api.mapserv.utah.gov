using ugrc.api.Cache;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Features.Geocoding;
public class PoBoxLocation {
    public class Computation(Address address, SingleGeocodeRequestOptionsContract options) : IComputation<Candidate?>, IHasGeocodingOptions {
        public readonly Address _address = address;

        public SingleGeocodeRequestOptionsContract Options { get; } = options;
    }

    public class Handler(IStaticCache cache, IComputeMediator computeMediator, ILogger log) : IComputationHandler<Computation, Candidate?> {
        private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions = cache.PoBoxExclusions;
        private readonly IComputeMediator _computeMediator = computeMediator;
        private readonly ILogger? _log = log?.ForContext<PoBoxLocation>();
        private readonly IDictionary<int, PoBoxAddress> _poBoxes = cache.PoBoxes;
        private readonly IReadOnlyCollection<int> _zipExclusions
         = cache.PoBoxZipCodesWithExclusions;

        public async Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (!request._address.Zip5.HasValue) {
                _log?.Debug("PoBox: no candidate");

                return null;
            }

            if (_poBoxes is null) {
                _log?.Warning("PoBox: cache is empty");

                return null;
            }

            if (!_poBoxes.ContainsKey(request._address.Zip5.Value)) {
                _log?.ForContext("zip", request._address.Zip5.Value)
                    .Debug("PoBox: cache miss");

                return null;
            }

            Candidate candidate;
            var key = (request._address.Zip5.Value * 10000) + request._address.PoBox;

            if (_zipExclusions.Any(x => x == request._address.Zip5) &&
                _exclusions.TryGetValue(key, out var value)) {
                _log?.ForContext("post office exclusion", key)
                    .Information("PoBox: exclusion match");

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
                _log?.Information("PoBox: point match");

                candidate = new Candidate(
                     request._address.StandardizedAddress(),
                     request._address.AddressGrids.FirstOrDefault()?.Grid ?? "unknown",
                     new Point(result.X, result.Y),
                     100,
                     "Post Office Point",
                     0
                );
            } else {
                return null;
            }

            if (request.Options.SpatialReference == 26912) {
                return candidate;
            }

            var projectedCandidate = await _computeMediator.Handle(
                new ProjectQuery.Computation(candidate, request.Options.SpatialReference), cancellationToken);

            return projectedCandidate;
        }
    }
}

public record PoBoxAddress(int Zip, double X, double Y);
public record PoBoxAddressCorrection(int ZipPlusFour, int Zip, double X, double Y) : PoBoxAddress(Zip, X, Y);
