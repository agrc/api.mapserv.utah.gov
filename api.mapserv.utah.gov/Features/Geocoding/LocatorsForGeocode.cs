using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Models.Constants;
using api.mapserv.utah.gov.Models.RequestOptions;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class LocatorsForGeocode {
        public class Command : IRequest<IReadOnlyCollection<LocatorProperties>> {
            internal readonly GeocodeAddress Address;
            internal readonly GeocodingOptions Options;

            public Command(GeocodeAddress address, GeocodingOptions options) {
                Address = address;
                Options = options;
            }
        }

        public class Handler : RequestHandler<Command, IReadOnlyCollection<LocatorProperties>> {
            private readonly List<LocatorConfiguration> _locators;
            private readonly ILogger _log;

            public Handler(IOptions<List<LocatorConfiguration>> options, ILogger log) {
                _log = log;
                _locators = options.Value;
            }

            private static IReadOnlyCollection<GeocodeInput> BuildAddressPermutations(
                GeocodeAddress address, int spatialReference) {
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
                GeocodeAddress address, IReadOnlyCollection<GeocodeInput> permutations, LocatorType locatorType) {
                var locators = new List<LocatorProperties>();

                _log.Verbose("Finding locators for {address}", address);

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

                _log.Debug("Using {locators} for {address}", locators, address);

                return locators;
            }

            private IReadOnlyCollection<LocatorProperties> LocatorProperties(IReadOnlyCollection<GeocodeInput> permutations,
             bool reversal, bool likelyReversal, LocatorType locatorType) {
                var locatorsForAddress = new List<LocatorProperties>();
                var locators = _locators.Where(x => x.LocatorType == locatorType);

                if (reversal) {
                    foreach (var locator in locators) {
                        locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.AddressInfo.ReversalAddress)));
                    }

                    return locatorsForAddress;
                }

                if (likelyReversal) {
                    foreach (var locator in locators) {
                        locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.AddressInfo.ReversalAddress)));
                    }
                }

                foreach (var locator in locators) {
                    locatorsForAddress.AddRange(permutations.Select(p => locator.ToLocatorProperty(p, (a) => a.Address)));
                }

                return locatorsForAddress;
            }

            protected override IReadOnlyCollection<LocatorProperties> Handle(Command request) {
                var permutations = BuildAddressPermutations(request.Address, request.Options.SpatialReference);

                return BuildLocatorLookup(request.Address, permutations, request.Options.Locators);
            }
        }
    }
}
