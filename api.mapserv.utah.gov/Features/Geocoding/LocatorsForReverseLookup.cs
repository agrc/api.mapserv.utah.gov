using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class LocatorsForReverseLookup {
        public class Command : IRequest<IReadOnlyCollection<LocatorProperties>> {
            public double _x;
            public double _y;
            public double _distance;
            public int _spatialReference;

            public Command(double x, double y, double distance, int spatialReference) {
                _x = x;
                _y = y;
                _distance = distance;
                _spatialReference = spatialReference;
            }
        }

        public class Handler : RequestHandler<Command, IReadOnlyCollection<LocatorProperties>> {
            private readonly List<ReverseLocatorConfiguration> _locators;

            public Handler(IOptions<List<ReverseLocatorConfiguration>> options) {
                _locators = options.Value;
            }

            protected override IReadOnlyCollection<LocatorProperties> Handle(Command request) {
                var locators = _locators.Where(x => x.ReverseGeocodes);
                var locatorsForLocation = new List<LocatorProperties>();

                foreach (var locator in locators) {
                    locatorsForLocation.Add(locator.ToLocatorProperty(request._x, request._y, request._distance, request._spatialReference));
                }

                return locatorsForLocation;
            }
        }
    }
}
