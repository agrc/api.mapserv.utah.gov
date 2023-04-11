using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Features.Geocoding;
public class UspsDeliveryPointLocation {
    public class Computation : IComputation<Candidate?>, IHasGeocodingOptions {
        internal readonly Address Address;

        public Computation(Address address, SingleGeocodeRequestOptionsContract options) {
            Address = address;
            Options = options;
        }

        public SingleGeocodeRequestOptionsContract Options { get; }
    }

    public class Handler : IComputationHandler<Computation, Candidate?> {
        private readonly IStaticCache _staticCache;
        private readonly ILogger? _log;

        public Handler(IStaticCache staticCache, ILogger log) {
            _staticCache = staticCache;
            _log = log?.ForContext<UspsDeliveryPointLocation>();
        }

        public Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (!request.Address.Zip5.HasValue) {
                _log?.Debug("no candidate", request.Address);

                return Task.FromResult<Candidate?>(null);
            }

            _staticCache.UspsDeliveryPoints.TryGetValue(request.Address.Zip5.Value.ToString(), out var items);

            if (items?.Any() != true) {
                _log?.ForContext("zip", request.Address.Zip5.Value)
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
