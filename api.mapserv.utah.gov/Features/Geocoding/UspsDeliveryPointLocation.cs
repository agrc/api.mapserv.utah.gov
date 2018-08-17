using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.GeometryService;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.Linkables;
using api.mapserv.utah.gov.Models.RequestOptions;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class UspsDeliveryPointLocation {
        public class Command : IRequest<Candidate> {
            internal readonly GeocodeAddress Address;
            internal readonly GeocodingOptions Options;

            public Command(GeocodeAddress address, GeocodingOptions options) {
                Address = address;
                Options = options;
            }
        }

        public class Handler : IRequestHandler<Command, Candidate> {
            private readonly ILookupCache _driveCache;
            private readonly IMediator _mediator;

            public Handler(ILookupCache driveCache, IMediator mediator) {
                _driveCache = driveCache;
                _mediator = mediator;
            }

            public async Task<Candidate> Handle(Command request, CancellationToken cancellationToken) {
                Log.Verbose("Testing for delivery points");

                if (!request.Address.Zip5.HasValue) {
                    Log.Debug("No delivery point for {address} because of no zip5", request.Address);

                    return null;
                }

                _driveCache.UspsDeliveryPoints.TryGetValue(request.Address.Zip5.Value.ToString(), out var items);

                if (items == null || !items.Any()) {
                    Log.Debug("No delivery point for {zip} in cache", request.Address.Zip5.Value);

                    return null;
                }

                if (!(items.FirstOrDefault() is UspsDeliveryPointLink deliveryPoint)) {
                    return null;
                }

                var result = new Candidate {
                    Address = deliveryPoint.MatchAddress,
                    AddressGrid = deliveryPoint.Grid,
                    Locator = "USPS Delivery Points",
                    Score = 100,
                    Location = new Point(deliveryPoint.X, deliveryPoint.Y)
                };

                Log.Information("Found delivery point for {address}", request.Address);

                if (request.Options.SpatialReference == 26912) {
                    return result;
                }

                Log.Debug("Reprojecting delivery point to {wkid}", request.Options.SpatialReference);

                var reproject =
                    new Reproject.Command(new PointReprojectOptions(26912, request.Options.SpatialReference,
                                                                    new[] {
                                                                        deliveryPoint.X,
                                                                        deliveryPoint.Y
                                                                    }));

                var pointReprojectResponse = await _mediator.Send(reproject, cancellationToken);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any()) {
                    Log.Fatal("Could not reproject point for {address}", request.Address);

                    return null;
                }

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null) {
                    result.Location = new Point(points.X, points.Y);
                }

                return result;
            }
        }
    }
}
