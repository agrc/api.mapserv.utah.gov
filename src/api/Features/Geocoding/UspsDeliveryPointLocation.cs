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

    public class Handler(IStaticCache staticCache, IComputeMediator computeMediator, ILogger log) : IComputationHandler<Computation, Candidate?> {
        private readonly IStaticCache _staticCache = staticCache;
        private readonly IComputeMediator _computeMediator = computeMediator;
        private readonly ILogger? _log = log?.ForContext<UspsDeliveryPointLocation>();

        public async Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (!request._address.Zip5.HasValue) {
                _log?.Debug("Delivery Point: no candidate", request._address);

                return null;
            }

            _staticCache.UspsDeliveryPoints.TryGetValue(request._address.Zip5.Value.ToString(), out var items);

            if ((items?.Count ?? 0) == 0) {
                _log?.ForContext("zip", request._address.Zip5.Value)
                    .Debug("Delivery Point: cache miss");

                return null;
            }

            if (items?.FirstOrDefault() is not UspsDeliveryPointLink deliveryPoint) {
                return null;
            }

            var candidate = new Candidate(
                deliveryPoint.MatchAddress,
                deliveryPoint.Grid,
                new Point(deliveryPoint.X, deliveryPoint.Y),
                100,
                "USPS Delivery Points",
            0
            );

            _log?.ForContext("delivery point", deliveryPoint.MatchAddress)
                .Information("Delivery Point: match");

            if (request.Options.SpatialReference == 26912) {
                return candidate;
            }

            var projectedCandidate = await _computeMediator.Handle(
                new ProjectQuery.Computation(candidate, request.Options.SpatialReference), cancellationToken);

            return projectedCandidate;
        }
    }
}
