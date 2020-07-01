using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Models.Constants;
using api.mapserv.utah.gov.Models.RequestOptions;
using Microsoft.Extensions.Options;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class GeocodePlan {
        public class Computation : IComputation<IReadOnlyCollection<LocatorProperties>> {
            internal readonly AddressWithGrids Address;
            internal readonly GeocodingOptions Options;

            public Computation(AddressWithGrids address, GeocodingOptions options) {
                Address = address;
                Options = options;
            }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<LocatorProperties>> {
            private readonly List<LocatorConfiguration> _locators;
            private readonly ILogger _log;

            public Handler(IOptions<List<LocatorConfiguration>> options, ILogger log) {
                _locators = options.Value;
                _log = log?.ForContext<GeocodePlan>();
            }

            private static IReadOnlyCollection<GeocodeInput> BuildAddressPermutations(
                AddressWithGrids address, int spatialReference) {
                var addressPermutations = new List<GeocodeInput>();

                if (!address.AddressGrids.Any()) {
                    return Array.Empty<GeocodeInput>();
                }

                foreach (var grid in address.AddressGrids) {
                    addressPermutations.Add(new GeocodeInput(address, grid.Grid, grid.Weight, null, spatialReference));
                }

                return addressPermutations;
            }

            private IReadOnlyCollection<LocatorProperties> BuildLocatorLookup(
                AddressWithGrids address, IReadOnlyCollection<GeocodeInput> permutations, LocatorType locatorType) {
                var locators = new List<LocatorProperties>();

                if (locatorType == LocatorType.Default || locatorType == LocatorType.All) {
                    locators.AddRange(
                         LocatorProperties(permutations, address.IsReversal(), address.PossibleReversal(), LocatorType.AddressPoints)
                     );

                    locators.AddRange(
                        LocatorProperties(permutations, address.IsReversal(), address.PossibleReversal(), LocatorType.RoadCenterlines)
                    );

                    return locators;
                }

                locators.AddRange(
                    LocatorProperties(permutations, address.IsReversal(), address.PossibleReversal(), locatorType)
                );

                _log.ForContext("address", address.StandardizedAddress)
                    .ForContext("urls", string.Join(",", locators.Select(x => x.Url)))
                    .Debug("geocode plan created");

                return locators;
            }

            private IReadOnlyCollection<LocatorProperties> LocatorProperties(IReadOnlyCollection<GeocodeInput> permutations,
             bool reversal, bool likelyReversal, LocatorType locatorType) {
                var locatorsForAddress = new List<LocatorProperties>();
                var locators = _locators.Where(x => x.LocatorType == locatorType);

                if (reversal || likelyReversal) {
                    foreach (var locator in locators) {
                        locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.AddressInfo.ReversalAddress)));
                    }
                }

                foreach (var locator in locators) {
                    locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.Address)));
                }

                return locatorsForAddress;
            }

            public Task<IReadOnlyCollection<LocatorProperties>> Handle(Computation request, CancellationToken cancellation) {
                var permutations = BuildAddressPermutations(request.Address, request.Options.SpatialReference);

                return Task.FromResult(BuildLocatorLookup(request.Address, permutations, request.Options.Locators));
            }
        }
    }
}
