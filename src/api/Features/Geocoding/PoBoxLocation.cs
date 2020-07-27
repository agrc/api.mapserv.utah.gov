using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class PoBoxLocation {
        public class Computation : IComputation<Candidate>, IHasGeocodingOptions {
            internal readonly AddressWithGrids Address;

            public Computation(AddressWithGrids address, SingleGeocodeRequestOptionsContract options) {
                Address = address;
                Options = options;
            }

            public SingleGeocodeRequestOptionsContract Options { get; }
        }

        public class Handler : IComputationHandler<Computation, Candidate> {
            private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions;
            private readonly ILogger _log;
            private readonly IDictionary<int, PoBoxAddress> _poBoxes;
            private readonly IReadOnlyCollection<int> _zipExclusions;

            public Handler(ILookupCache driveCache, ILogger log) {
                _poBoxes = driveCache.PoBoxes;
                _exclusions = driveCache.PoBoxExclusions;
                _zipExclusions = driveCache.PoBoxZipCodesWithExclusions;
                _log = log?.ForContext<PoBoxLocation>();
            }

            public Task<Candidate> Handle(Computation request, CancellationToken cancellationToken) {
                if (!request.Address.Zip5.HasValue) {
                    _log.Debug("no candidate");

                    return Task.FromResult((Candidate)null);
                }

                if (_poBoxes is null) {
                    _log.Warning("cache is empty");

                    return Task.FromResult((Candidate)null);
                }

                if (!_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                    _log.ForContext("zip", request.Address.Zip5.Value)
                        .Debug("cache miss");

                    return Task.FromResult((Candidate)null);
                }

                Candidate candidate;
                var key = request.Address.Zip5.Value * 10000 + request.Address.PoBox;

                if (_zipExclusions.Any(x => x == request.Address.Zip5) &&
                    _exclusions.ContainsKey(key)) {
                    _log.ForContext("post office exclusion", key)
                        .Information("match");

                    var exclusion = _exclusions[key];
                    candidate = new Candidate {
                        Address = request.Address.StandardizedAddress,
                        Locator = "Post Office Point Exclusions",
                        Score = 100,
                        Location = new Point(exclusion.X, exclusion.Y),
                        AddressGrid = request.Address?.AddressGrids?.FirstOrDefault()?.Grid
                    };
                } else if (_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                    _log.Information("match");

                    var result = _poBoxes[request.Address.Zip5.Value];
                    candidate = new Candidate {
                        Address = request.Address.StandardizedAddress,
                        Locator = "Post Office Point",
                        Score = 100,
                        Location = new Point(result.X, result.Y),
                        AddressGrid = request.Address.AddressGrids.FirstOrDefault()?.Grid
                    };
                } else {
                    return Task.FromResult((Candidate)null);
                }

                return Task.FromResult(candidate);
            }
        }
    }
}
