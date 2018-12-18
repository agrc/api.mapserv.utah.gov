using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.GeometryService;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.RequestOptions;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class PoBoxLocation {
        public class Command : IHasGeocodingOptions, IRequest<Candidate> {
            internal readonly AddressWithGrids Address;

            public Command(AddressWithGrids address, GeocodingOptions options) {
                Address = address;
                Options = options;
            }

            public GeocodingOptions Options { get; }
        }

        public class Handler : IRequestHandler<Command, Candidate> {
            private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions;
            private readonly ILogger _log;

            private readonly IDictionary<int, PoBoxAddress> _poBoxes;
            private readonly IReadOnlyCollection<int> _zipExclusions;

            public Handler(ILookupCache driveCache, ILogger log) {
                _poBoxes = driveCache.PoBoxes;
                _exclusions = driveCache.PoBoxExclusions;
                _zipExclusions = driveCache.PoBoxZipCodesWithExclusions;
                _log = log;
            }

            public Task<Candidate> Handle(Command request, CancellationToken cancellationToken) {
                if (!request.Address.Zip5.HasValue) {
                    _log.Debug("No zip code, can't be po box {address}", request.Address);

                    return Task.FromResult((Candidate)null);
                }

                if (_poBoxes is null) {
                    _log.Warning("Po Box cache is empty!");

                    return Task.FromResult((Candidate)null);
                }

                if (!_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                    _log.Debug("{zip} is not in the po box cache", request.Address.Zip5.Value);

                    return Task.FromResult((Candidate)null);
                }

                Candidate candidate;
                var key = request.Address.Zip5.Value * 10000 + request.Address.PoBox;

                if (_zipExclusions.Any(x => x == request.Address.Zip5) &&
                    _exclusions.ContainsKey(key)) {
                    _log.Information("{Using Post Office Point Exclusion for {zip}", key);

                    var exclusion = _exclusions[key];
                    candidate = new Candidate {
                        Address = request.Address.StandardizedAddress,
                        Locator = "Post Office Point Exclusions",
                        Score = 100,
                        Location = new Point(exclusion.X, exclusion.Y),
                        AddressGrid = request.Address?.AddressGrids?.FirstOrDefault()?.Grid
                    };
                } else if (_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                    _log.Information("Using post office point for {zip}", key);

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
