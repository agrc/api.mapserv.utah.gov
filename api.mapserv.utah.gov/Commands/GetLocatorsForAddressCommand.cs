using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.Extensions.Options;

namespace api.mapserv.utah.gov.Commands
{
  public class GetLocatorsForAddressCommand : Command<ReadOnlyCollection<LocatorProperties>>
    {
        public GetLocatorsForAddressCommand(IOptions<GisServerConfiguration> options)
        {
            Host = options.Value.Host;
        }

        public void Initialize(GeocodeAddress address, GeocodingOptions options)
        {
            Address = address;
            Options = options;

            BuildAddressPermutations();
            BuildLocatorLookup();
        }

        public GeocodeAddress Address { get; set; }

        public string Host { get; set; }

        public GeocodingOptions Options { get; set; }

        private IDictionary<LocatorType, List<LocatorProperties>> LocatorLookup { get; set; }

        protected List<GeocodeInput> AddressPermutations { get; set; }

        private void BuildAddressPermutations()
        {
            AddressPermutations = new List<GeocodeInput>();

            if (!Address.AddressGrids.Any())
            {
                return;
            }

            foreach (var grid in Address.AddressGrids)
            {
                AddressPermutations.Add(new GeocodeInput(Address, grid.Grid, grid.Weight, null, Options.WkId));
            }
        }

        private void BuildLocatorLookup()
        {
            var locatorLookup = new Dictionary<LocatorType, List<LocatorProperties>>();

            Add(UspsDeliveryPoint(), ref locatorLookup, LocatorType.All);
            Add(Intersection(), ref locatorLookup, LocatorType.All);
            Add(AddressPoints(), ref locatorLookup, LocatorType.AddressPoints);
            Add(Reversal(Address), ref locatorLookup, LocatorType.RoadCenterlines);
            Add(Centerlines(Address), ref locatorLookup, LocatorType.RoadCenterlines);

            LocatorLookup = locatorLookup;
        }

        private IEnumerable<LocatorProperties> Centerlines(AddressBase address)
        {
            var locators = new List<LocatorProperties>();

            if (address.IsReversal())
            {
                AddressPermutations.ForEach(stuff => locators.AddRange(new[]
                {
                    new LocatorProperties
                    {
                        Url = $"http://{Host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                              $"GeocodeServer/findAddressCandidates?f=json&Street={HttpUtility.UrlEncode(stuff.Address)}" +
                              $"&City={stuff.Grid}&outSR={stuff.WkId}",
                        Name = "Centerlines.StatewideRoads",
                        Weight = stuff.Weight
                    }
                }));

                return locators;
            }

            AddressPermutations.ForEach(stuff => locators.AddRange(new[]
            {
                new LocatorProperties
                {
                    Url = $"http://{Host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                          $"GeocodeServer/findAddressCandidates?f=json&Street={HttpUtility.UrlEncode(stuff.Address)}" +
                          $"&City={stuff.Grid}&outSR={stuff.WkId}",
                    Name = "Centerlines.StatewideRoads",
                    Weight = stuff.Weight
                }
            }));

            return locators;
        }

        private IEnumerable<LocatorProperties> AddressPoints()
        {
            var locators = new List<LocatorProperties>();

            AddressPermutations.ForEach(x =>
            {
                if (x.AddressInfo.IsReversal() || x.AddressInfo.PossibleReversal())
                {
                    locators.Add(new LocatorProperties
                    {
                        Url = $"http://{Host}/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                              $"GeocodeServer/findAddressCandidates?f=json&Street={HttpUtility.UrlEncode(x.AddressInfo.ReversalAddress)}" +
                              $"&City={x.Grid}&outSR={x.WkId}",
                        Name = "AddressPoints.AddressGrid",
                        Weight = x.Weight
                    });
                }

                locators.Add(new LocatorProperties
                {
                    Url = $"http://{Host}/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                          $"GeocodeServer/findAddressCandidates?f=json&Street={HttpUtility.UrlEncode(x.Address)}" +
                          $"&City={x.Grid}&outSR={x.WkId}",
                    Name = "AddressPoints.AddressGrid",
                    Weight = x.Weight
                });
            });

            return locators;
        }

        /// <summary>
        ///     Determines if the address is likely a reversal
        ///     case 1: (300 S 437 E) where the street name ends in 2,3,4,6,7,8,9
        ///     case 2: (350 S 435 E) where street name and house number both end in a 0 or 5
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>locator_SwitchRoadandHouseNumber</returns>
        private IEnumerable<LocatorProperties> Reversal(AddressBase address)
        {
            if (!address.IsReversal() && !address.PossibleReversal())
            {
                return Enumerable.Empty<LocatorProperties>();
            }

            var locators = new List<LocatorProperties>();

            AddressPermutations.ForEach(stuff => locators.AddRange(new[]
            {
                new LocatorProperties
                {
                    Url = $"http://{Host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                          $"GeocodeServer/findAddressCandidates?f=json&Street={HttpUtility.UrlEncode(stuff.AddressInfo.ReversalAddress)}&City={stuff.Grid}&outSR={stuff.WkId}",
                    Name = "Centerlines.StatewideRoads"
                }
            }));

            return locators;
        }

        /// <summary>
        ///     Determines if the address is an intersection.
        ///     If the word and is found in the address this is true
        /// </summary>
        /// <returns>locator_IntersectionPoints</returns>
        private static IEnumerable<LocatorProperties> Intersection()
        {
            return Enumerable.Empty<LocatorProperties>();
        }

        /// <summary>
        ///     If the address is a usps delivery point the usps locator will be added
        /// </summary>
        /// <returns>locator_DeliveryPoints</returns>
        private static IEnumerable<LocatorProperties> UspsDeliveryPoint()
        {
            return Enumerable.Empty<LocatorProperties>();
        }

        /// <summary>
        ///     Adds the specified locator to the locators list if the return value is not null.
        /// </summary>
        /// <param name="locatorsToAdd">The locator.</param>
        /// <param name="lookup">The lookup.</param>
        /// <param name="key">The key.</param>
        private static void Add(IEnumerable<LocatorProperties> locatorsToAdd,
                                ref Dictionary<LocatorType, List<LocatorProperties>> lookup, LocatorType key)
        {
            if (locatorsToAdd == null)
            {
                return;
            }

            locatorsToAdd = locatorsToAdd.ToList();

            if (!locatorsToAdd.Any())
            {
                return;
            }

            if (!lookup.ContainsKey(key))
            {
                lookup.Add(key, locatorsToAdd.ToList());
            }
            else
            {
                lookup[key].AddRange(locatorsToAdd);
            }
        }

        public override string ToString() => $"GetAltNameLocatorsCommand, Address: {Address}";

        protected override void Execute()
        {
            if (!Address.IsMachable())
            {
                return;
            }

            switch (Options.Locators)
            {
                case LocatorType.Default:
                case LocatorType.All:
                    Result = LocatorLookup.Values.SelectMany(x => x).ToList().AsReadOnly();
                    break;
                case LocatorType.AddressPoints:
                    if (!LocatorLookup.ContainsKey(Options.Locators))
                    {
                        break;
                    }

                    Result = LocatorLookup[LocatorType.AddressPoints].ToList().AsReadOnly();
                    break;
                case LocatorType.RoadCenterlines:
                    if (!LocatorLookup.ContainsKey(Options.Locators))
                    {
                        break;
                    }

                    Result = LocatorLookup[LocatorType.RoadCenterlines].ToList().AsReadOnly();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("", Options.Locators, "Acceptable options are all, roadcenterlines, and addresspoints");
            }
        }
    }
}
