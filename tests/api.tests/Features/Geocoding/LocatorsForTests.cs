using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Models.Constants;
using api.mapserv.utah.gov.Models.Linkables;
using api.mapserv.utah.gov.Models.RequestOptions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class LocatorsForTests {
        public class LocatorsForReverseGeocodingTests {
            public LocatorsForReverseGeocodingTests() {
                var options = new Mock<IOptions<GisServerConfiguration>>();
                options.Setup(x => x.Value).Returns(new GisServerConfiguration {
                    Host = "test",
                    Port = "1",
                    Protocol = "proto"
                });

                handler = new LocatorsForReverseLookup.Handler(options.Object);
            }

            internal IRequestHandler<LocatorsForReverseLookup.Command, IReadOnlyCollection<LocatorProperties>> handler;

            [Fact]
            public async Task Should_return_centerline_geocoder_only() {
                var request = new LocatorsForReverseLookup.Command();
                var options = new Mock<IOptions<GisServerConfiguration>>();
                options.Setup(x => x.Value).Returns(new GisServerConfiguration {
                    Host = "test",
                    Port = "1",
                    Protocol = "proto"
                });

                var result = await handler.Handle(request, new CancellationToken());

                result.ShouldHaveSingleItem();

                var locator = result.First();
                locator.Url.ShouldBe("proto://test:1/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/reverseGeocode?location={0},{1}&distance={2}&outSR={3}&f=json");
                locator.Name.ShouldBe("Centerlines.StatewideRoads");
            }
        }

        public class LocatorsForGeocodingTests {
            public LocatorsForGeocodingTests() {
                var options = new Mock<IOptions<GisServerConfiguration>>();
                options.Setup(x => x.Value).Returns(new GisServerConfiguration {
                    Host = "test",
                    Port = "1",
                    Protocol = "proto"
                });

                handler = new LocatorsForGeocode.Handler(options.Object);
            }

            internal IRequestHandler<LocatorsForGeocode.Command, IReadOnlyCollection<LocatorProperties>> handler;

            [Fact]
            public async Task Should_create_extra_for_address_reversal() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "2", StreetType.Alley,
                                                        Direction.South, 0, 84114, false, false);
                var address = new GeocodeAddress(parsedAddress);
                address.AddressGrids = new[] {new PlaceGridLink("place", "grid", 0)};

                var geocodeOptions = new GeocodingOptions {
                    Locators = LocatorType.RoadCenterlines,
                    SpatialReference = 26912
                };

                var request = new LocatorsForGeocode.Command(address, geocodeOptions);
                var result = await handler.Handle(request, new CancellationToken());

                result.Count.ShouldBe(2);

                result.Count(x => x.Url ==
                                  "proto://test:1/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/findAddressCandidates?f=json&Street=1+North+2+Alley+South&City=grid&outSR=26912")
                      .ShouldBe(1);
                result.Count(x => x.Url ==
                                  "proto://test:1/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/findAddressCandidates?f=json&Street=2+South+Alley+1+North&City=grid&outSR=26912")
                      .ShouldBe(1);
            }

            [Fact]
            public async Task Should_return_address_point_geocoder_only() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new GeocodeAddress(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new GeocodingOptions {
                    Locators = LocatorType.AddressPoints,
                    SpatialReference = 26912
                };

                var request = new LocatorsForGeocode.Command(address, geocodeOptions);
                var result = await handler.Handle(request, new CancellationToken());

                result.ShouldHaveSingleItem();

                var locator = result.First();
                locator.Url.ShouldBe("proto://test:1/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/GeocodeServer/findAddressCandidates?f=json&Street=1+North+street+Alley+South&City=grid&outSR=26912");
                locator.Name.ShouldBe("AddressPoints.AddressGrid");
            }

            [Fact]
            public async Task Should_return_all_geocoders() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new GeocodeAddress(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new GeocodingOptions {
                    Locators = LocatorType.All,
                    SpatialReference = 26912
                };

                var request = new LocatorsForGeocode.Command(address, geocodeOptions);
                var result = await handler.Handle(request, new CancellationToken());

                result.Count.ShouldBe(2);

                geocodeOptions = new GeocodingOptions {
                    Locators = LocatorType.Default,
                    SpatialReference = 26912
                };

                request = new LocatorsForGeocode.Command(address, geocodeOptions);
                result = await handler.Handle(request, new CancellationToken());

                result.Count.ShouldBe(2);
            }

            [Fact]
            public async Task Should_return_centerline_geocoder_only() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new GeocodeAddress(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new GeocodingOptions {
                    Locators = LocatorType.RoadCenterlines,
                    SpatialReference = 26912
                };

                var request = new LocatorsForGeocode.Command(address, geocodeOptions);
                var result = await handler.Handle(request, new CancellationToken());

                result.ShouldHaveSingleItem();

                var locator = result.First();
                locator.Url.ShouldBe("proto://test:1/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/findAddressCandidates?f=json&Street=1+North+street+Alley+South&City=grid&outSR=26912");
                locator.Name.ShouldBe("Centerlines.StatewideRoads");
            }

            [Fact]
            public async Task Should_return_empty_when_no_grids() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new GeocodeAddress(parsedAddress) {
                    AddressGrids = Array.Empty<GridLinkable>()
                };

                var geocodeOptions = new GeocodingOptions {
                    Locators = LocatorType.RoadCenterlines,
                    SpatialReference = 26912
                };

                var request = new LocatorsForGeocode.Command(address, geocodeOptions);
                var result = await handler.Handle(request, new CancellationToken());

                result.ShouldBeEmpty();
            }
        }
    }
}
