using ugrc.api.Infrastructure;
using ugrc.api.Models.Constants;
using Microsoft.Extensions.Options;

namespace ugrc.api.Features.Geocoding;
public class GeocodePlan {
    public class Computation(Address address, SingleGeocodeRequestOptionsContract options) : IComputation<IReadOnlyCollection<LocatorProperties>> {
        public readonly Address _address = address;
        public readonly SingleGeocodeRequestOptionsContract _options = options;
    }

    public class Handler(IOptions<List<LocatorConfiguration>> options, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<LocatorProperties>> {
        private readonly List<LocatorConfiguration> _locators = options.Value;
        private readonly ILogger? _log = log?.ForContext<GeocodePlan>();

        private static IReadOnlyCollection<LocatorMetadata> BuildAddressPermutations(
            Address address, int spatialReference) {
            var addressPermutations = new List<LocatorMetadata>();

            if (address.AddressGrids.Count == 0) {
                return [];
            }

            foreach (var grid in address.AddressGrids) {
                addressPermutations.Add(new LocatorMetadata(address, grid.Grid, grid.Weight, null, spatialReference));
            }

            return addressPermutations;
        }

        private IReadOnlyCollection<LocatorProperties> BuildLocatorLookup(
            Address address, IReadOnlyCollection<LocatorMetadata> permutations, LocatorType locatorType) {
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

            _log?.ForContext("address", address.StandardizedAddress())
                .ForContext("urls", string.Join(",", locators.Select(x => x.Url)))
                .Debug("Geocode plan created");

            return locators;
        }

        private IReadOnlyCollection<LocatorProperties> LocatorProperties(IReadOnlyCollection<LocatorMetadata> permutations,
         bool reversal, bool likelyReversal, LocatorType locatorType) {
            var locatorsForAddress = new List<LocatorProperties>();
            var locators = _locators.Where(x => x.LocatorType == locatorType);

            if (reversal || likelyReversal) {
                foreach (var locator in locators) {
                    locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.AddressInfo.ReverseAddressParts())));
                }
            }

            foreach (var locator in locators) {
                locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.Address)));
            }

            return locatorsForAddress;
        }

        public Task<IReadOnlyCollection<LocatorProperties>> Handle(Computation request, CancellationToken cancellation) {
            var permutations = BuildAddressPermutations(request._address, request._options.SpatialReference);

            return Task.FromResult(BuildLocatorLookup(request._address, permutations, request._options.Locators));
        }
    }
}
