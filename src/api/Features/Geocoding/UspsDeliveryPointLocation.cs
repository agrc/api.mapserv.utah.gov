using ugrc.api.Cache;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Features.Geocoding;
public class UspsDeliveryPointLocation {
    public class Computation(Address address, SingleGeocodeRequestOptionsContract options) : IComputation<Candidate?>, IHasGeocodingOptions {
        public readonly Address _address = address;

        public SingleGeocodeRequestOptionsContract Options { get; } = options;
    }

    public class Handler(IStaticCache staticCache, ILogger log) : IComputationHandler<Computation, Candidate?> {
        private readonly IStaticCache _staticCache = staticCache;
        private readonly ILogger? _log = log?.ForContext<UspsDeliveryPointLocation>();

        public Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (!request._address.Zip5.HasValue) {
                _log?.Debug("Delivery Point: no candidate", request._address);

                return Task.FromResult<Candidate?>(null);
            }

            _staticCache.UspsDeliveryPoints.TryGetValue(request._address.Zip5.Value.ToString(), out var items);

            if ((items?.Count ?? 0) == 0) {
                _log?.ForContext("zip", request._address.Zip5.Value)
                    .Debug("Delivery Point: cache miss");

                return Task.FromResult<Candidate?>(null);
            }

            if (items.FirstOrDefault() is not UspsDeliveryPointLink deliveryPoint) {
                return Task.FromResult<Candidate?>(null);
            }

            var result = new Candidate(
                deliveryPoint.MatchAddress,
                deliveryPoint.Grid,
                new Point(deliveryPoint.X, deliveryPoint.Y),
                100,
                "USPS Delivery Points",
            0
            );

            _log?.ForContext("delivery point", deliveryPoint.MatchAddress)
                .Information("Delivery Point: match");

            return Task.FromResult<Candidate?>(result);
        }
    }
}
