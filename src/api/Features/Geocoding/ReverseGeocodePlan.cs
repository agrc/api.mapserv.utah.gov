using Microsoft.Extensions.Options;
using ugrc.api.Infrastructure;
using ugrc.api.Models;

namespace ugrc.api.Features.Geocoding;
public static class ReverseGeocodePlan {
    public class Computation(PointWithSpatialReference point, double distance, int spatialReference) : IComputation<IReadOnlyCollection<LocatorProperties>> {
        public PointWithSpatialReference _point = point;
        public double _distance = distance;
        public int _spatialReference = spatialReference;
    }

    public class Handler(IOptions<List<ReverseLocatorConfiguration>> options) : IComputationHandler<Computation, IReadOnlyCollection<LocatorProperties>> {
        private readonly List<ReverseLocatorConfiguration> _locators = options.Value;

        public Task<IReadOnlyCollection<LocatorProperties>> Handle(Computation request, CancellationToken cancellationToken) {
            var locators = _locators.Where(x => x.ReverseGeocodes);
            var locatorsForLocation = new List<LocatorProperties>();

            foreach (var locator in locators) {
                locatorsForLocation.Add(locator.ToLocatorProperty(request._point, request._distance, request._spatialReference));
            }

            return Task.FromResult<IReadOnlyCollection<LocatorProperties>>(locatorsForLocation);
        }
    }
}
