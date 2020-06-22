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
        public class Command : IHasGeocodingOptions, IRequest<Candidate> {
            internal readonly AddressWithGrids Address;

            public Command(AddressWithGrids address, GeocodingOptions options) {
                Address = address;
                Options = options;
            }

            public GeocodingOptions Options { get; }
        }

        public class Handler : IRequestHandler<Command, Candidate> {
            private readonly ILookupCache _driveCache;
            private readonly ILogger _log;

            public Handler(ILookupCache driveCache, ILogger log) {
                _driveCache = driveCache;
                _log = log?.ForContext<UspsDeliveryPointLocation>();
            }

            public Task<Candidate> Handle(Command request, CancellationToken cancellationToken) {
                _log.Verbose("Testing for delivery points");

                if (!request.Address.Zip5.HasValue) {
                    _log.Debug("No delivery point for {address} because of no zip5", request.Address);

                    return Task.FromResult((Candidate)null);
                }

                _driveCache.UspsDeliveryPoints.TryGetValue(request.Address.Zip5.Value.ToString(), out var items);

                if (items == null || !items.Any()) {
                    _log.Debug("No delivery point for {zip} in cache", request.Address.Zip5.Value);

                    return Task.FromResult((Candidate)null);
                }

                if (!(items.FirstOrDefault() is UspsDeliveryPointLink deliveryPoint)) {
                    return Task.FromResult((Candidate)null);
                }

                var result = new Candidate {
                    Address = deliveryPoint.MatchAddress,
                    AddressGrid = deliveryPoint.Grid,
                    Locator = "USPS Delivery Points",
                    Score = 100,
                    Location = new Point(deliveryPoint.X, deliveryPoint.Y)
                };

                _log.Information("Found delivery point for {address}", request.Address);

                return Task.FromResult(result);
            }
        }
    }
}
