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
        public class Command : IRequest<Candidate> {
            internal readonly GeocodeAddress Address;
            internal readonly GeocodingOptions Options;

            public Command(GeocodeAddress address, GeocodingOptions options) {
                Address = address;
                Options = options;
            }
        }

        public class Handler : IRequestHandler<Command, Candidate> {
            private readonly IDictionary<int, PoBoxAddress> _poBoxes;
            private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions;
            private readonly IReadOnlyCollection<int> _zipExclusions;

            private readonly IMediator _mediator;

            public Handler(ILookupCache driveCache, IMediator mediator) {
                _poBoxes = driveCache.PoBoxes;
                _exclusions = driveCache.PoBoxExclusions;
                _zipExclusions = driveCache.PoBoxZipCodesWithExclusions;
                _mediator = mediator;
            }

            public async Task<Candidate> Handle(Command request, CancellationToken cancellationToken) {
                if (!request.Address.Zip5.HasValue) {
                    Log.Debug("No zip code, can't be po box {address}", request.Address);

                    return null;
                }

                if (_poBoxes is null) {
                    Log.Warning("Po Box cache is empty!");

                    return null;
                }

                if (!_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                    Log.Debug("{zip} is not in the po box cache", request.Address.Zip5.Value);

                    return null;
                }

                Candidate candidate;
                var key = request.Address.Zip5.Value * 10000 + request.Address.PoBox;

                if (_zipExclusions.Any(x => x == request.Address.Zip5) &&
                    _exclusions.ContainsKey(key)) {
                    Log.Information("{Using Post Office Point Exclusion for {zip}", key);

                    var exclusion = _exclusions[key];
                    candidate = new Candidate {
                        Address = request.Address.StandardizedAddress,
                        Locator = "Post Office Point Exclusions",
                        Score = 100,
                        Location = new Point(exclusion.X, exclusion.Y),
                        AddressGrid = request.Address?.AddressGrids?.FirstOrDefault()?.Grid
                    };
                } else if (_poBoxes.ContainsKey(request.Address.Zip5.Value)) {
                    Log.Information("Using post office point for {zip}", key);

                    var result = _poBoxes[request.Address.Zip5.Value];
                    candidate = new Candidate {
                        Address = request.Address.StandardizedAddress,
                        Locator = "Post Office Point",
                        Score = 100,
                        Location = new Point(result.X, result.Y),
                        AddressGrid = request.Address.AddressGrids.FirstOrDefault()?.Grid
                    };
                } else {
                    return null;
                }

                if (request.Options.SpatialReference == 26912) {
                    return candidate;
                }

                var reprojectCommand =
                    new Reproject.Command(new PointReprojectOptions(26912, request.Options.SpatialReference,
                                                                    new[] {
                                                                        candidate.Location.X,
                                                                        candidate.Location.Y
                                                                    }));

                var pointReprojectResponse = await _mediator.Send(reprojectCommand, cancellationToken);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any()) {
                    Log.Fatal("Could not reproject point for {candidate}", candidate);

                    return null;
                }

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null) {
                    candidate.Location = new Point(points.X, points.Y);
                }

                return candidate;
            }
        }
    }
}
