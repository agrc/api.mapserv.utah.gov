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
            private readonly string _host;

            public Handler(IOptions<GisServerConfiguration> options) {
                _host = options.Value.ToString();
            }

            private static IReadOnlyCollection<GeocodeInput> BuildAddressPermutations(GeocodeAddress address, int spatialReference) {
                var addressPermutations = new List<GeocodeInput>();

                if (!address.AddressGrids.Any()) {
                    return Array.Empty<GeocodeInput>();
                }

                foreach (var grid in address.AddressGrids) {
                    addressPermutations.Add(new GeocodeInput(address, grid.Grid, grid.Weight, null, spatialReference));
                }

                return addressPermutations;
            }

            private Dictionary<LocatorType, IReadOnlyCollection<LocatorProperties>> BuildLocatorLookup(GeocodeAddress address, IReadOnlyCollection<GeocodeInput> permutations) {
                var locatorLookup = new Dictionary<LocatorType, IReadOnlyCollection<LocatorProperties>>();

                Log.Verbose("Finding locators for {address}", address);

                Add(Intersection(), ref locatorLookup, LocatorType.All);
                Add(AddressPoints(permutations), ref locatorLookup, LocatorType.AddressPoints);
                Add(Reversal(address.IsReversal(), address.PossibleReversal(), permutations), ref locatorLookup, LocatorType.RoadCenterlines);
                Add(Centerlines(address.IsReversal(), permutations), ref locatorLookup, LocatorType.RoadCenterlines);

                Log.Debug("Using {locators} for {address}", locatorLookup, address);

                return locatorLookup;
            }

            /// <summary>
            ///     Adds the specified locator to the locators list if the return value is not null.
            /// </summary>
            /// <param name="locatorsToAdd">The locator.</param>
            /// <param name="lookup">The lookup.</param>
            /// <param name="key">The key.</param>
            private static void Add(IReadOnlyCollection<LocatorProperties> locatorsToAdd,
                                    ref Dictionary<LocatorType, IReadOnlyCollection<LocatorProperties>> lookup, LocatorType key) {
                if (locatorsToAdd == null) {
                    return;
                }

                locatorsToAdd = locatorsToAdd.ToList();

                if (!locatorsToAdd.Any()) {
                    return;
                }

                if (!lookup.ContainsKey(key)) {
                    lookup.Add(key, locatorsToAdd);
                } else {
                    lookup[key] = lookup[key].Concat(locatorsToAdd).ToArray();
                }
            }

            private IReadOnlyCollection<LocatorProperties> Centerlines(bool reversal, IReadOnlyCollection<GeocodeInput> permutations) {
                var locators = new List<LocatorProperties>();

                if (reversal) {
                    locators.AddRange(permutations.Select(permutation => new LocatorProperties {
                        Url = $"{_host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" + $"GeocodeServer/findAddressCandidates?f=json&Street={WebUtility.UrlEncode(permutation.Address)}" + $"&City={permutation.Grid}&outSR={permutation.WkId}",
                        Name = "Centerlines.StatewideRoads",
                        Weight = permutation.Weight
                    }));

                    return locators;
                }

                locators.AddRange(permutations.Select(permutation => new LocatorProperties {
                    Url = $"{_host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" + $"GeocodeServer/findAddressCandidates?f=json&Street={WebUtility.UrlEncode(permutation.Address)}" + $"&City={permutation.Grid}&outSR={permutation.WkId}",
                    Name = "Centerlines.StatewideRoads",
                    Weight = permutation.Weight
                }));

                return locators;
            }

            private IReadOnlyCollection<LocatorProperties> AddressPoints(IReadOnlyCollection<GeocodeInput> permutations) {
                var locators = new List<LocatorProperties>();

                foreach (var permutation in permutations) {
                    if (permutation.AddressInfo.IsReversal() || permutation.AddressInfo.PossibleReversal()) {
                        locators.Add(new LocatorProperties {
                            Url = $"{_host}/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                                  $"GeocodeServer/findAddressCandidates?f=json&Street={WebUtility.UrlEncode(permutation.AddressInfo.ReversalAddress)}" +
                                  $"&City={permutation.Grid}&outSR={permutation.WkId}",
                            Name = "AddressPoints.AddressGrid",
                            Weight = permutation.Weight
                        });
                    }

                    locators.Add(new LocatorProperties {
                        Url = $"{_host}/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                              $"GeocodeServer/findAddressCandidates?f=json&Street={WebUtility.UrlEncode(permutation.Address)}" +
                              $"&City={permutation.Grid}&outSR={permutation.WkId}",
                        Name = "AddressPoints.AddressGrid",
                        Weight = permutation.Weight
                    });
                }

                return locators;
            }

            /// <summary>
            ///     Determines if the address is likely a reversal
            ///     case 1: (300 S 437 E) where the street name ends in 2,3,4,6,7,8,9
            ///     case 2: (350 S 435 E) where street name and house number both end in a 0 or 5
            /// </summary>
            private IReadOnlyCollection<LocatorProperties> Reversal(bool reversal, bool possibleReversal, IReadOnlyCollection<GeocodeInput> permutations) {
                if (!reversal && !possibleReversal) {
                    return Array.Empty<LocatorProperties>();
                }

                return permutations.Select(permutation => new LocatorProperties {
                    Url = $"{_host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" + $"GeocodeServer/findAddressCandidates?f=json&Street={WebUtility.UrlEncode(permutation.AddressInfo.ReversalAddress)}&City={permutation.Grid}&outSR={permutation.WkId}",
                    Name = "Centerlines.StatewideRoads"
                })
                                   .ToList();
            }

            /// <summary>
            ///     Determines if the address is an intersection.
            ///     If the word and is found in the address this is true
            /// </summary>
            /// <returns>locator_IntersectionPoints</returns>
            private static IReadOnlyCollection<LocatorProperties> Intersection() => Array.Empty<LocatorProperties>();

            protected override IReadOnlyCollection<LocatorProperties> Handle(Command request) {
                var permutations = BuildAddressPermutations(request.Address, request.Options.SpatialReference);
                var locators = BuildLocatorLookup(request.Address, permutations);

                switch (request.Options.Locators) {
                    case LocatorType.Default:
                    case LocatorType.All:
                        return locators.Values.SelectMany(x => x).ToList().AsReadOnly();
                    case LocatorType.AddressPoints:
                        if (!locators.ContainsKey(request.Options.Locators)) {
                            break;
                        }

                        return locators[LocatorType.AddressPoints].ToList().AsReadOnly();
                    case LocatorType.RoadCenterlines:
                        if (!locators.ContainsKey(request.Options.Locators)) {
                            break;
                        }

                        return locators[LocatorType.RoadCenterlines].ToList().AsReadOnly();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.Options.Locators), request.Options.Locators, "Acceptable options are all, roadcenterlines, and addresspoints");
                }

                return Array.Empty<LocatorProperties>();
            }
        }
    }
}
