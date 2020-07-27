using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AGRC.api.Features.Geocoding {
    public class ReverseGeocodePlan {
        public class Computation : IComputation<IReadOnlyCollection<LocatorProperties>> {
            internal double _x;
            internal double _y;
            internal double _distance;
            internal int _spatialReference;

            public Computation(double x, double y, double distance, int spatialReference) {
                _x = x;
                _y = y;
                _distance = distance;
                _spatialReference = spatialReference;
            }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<LocatorProperties>> {
            private readonly List<ReverseLocatorConfiguration> _locators;

            public Handler(IOptions<List<ReverseLocatorConfiguration>> options) {
                _locators = options.Value;
            }

            public Task<IReadOnlyCollection<LocatorProperties>> Handle(Computation request, CancellationToken cancellationToken) {
                var locators = _locators.Where(x => x.ReverseGeocodes);
                var locatorsForLocation = new List<LocatorProperties>();

                foreach (var locator in locators) {
                    locatorsForLocation.Add(locator.ToLocatorProperty(request._x, request._y, request._distance, request._spatialReference));
                }

                return Task.FromResult<IReadOnlyCollection<LocatorProperties>>(locatorsForLocation);
            }
        }
    }
}
