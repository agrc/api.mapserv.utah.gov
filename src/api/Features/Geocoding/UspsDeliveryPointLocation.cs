using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Features.Geocoding;
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
                _log?.Debug("no candidate", request._address);

                return Task.FromResult<Candidate?>(null);
            }

            _staticCache.UspsDeliveryPoints.TryGetValue(request._address.Zip5.Value.ToString(), out var items);

            if (items?.Any() != true) {
                _log?.ForContext("zip", request._address.Zip5.Value)
                    .Debug("cache miss");

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
                .Information("match");

            return Task.FromResult<Candidate?>(result);
        }
    }
}
