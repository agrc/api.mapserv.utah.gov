using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using AGRC.api.Models.Linkables;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class LocatorsForTests {
        public class LocatorsForReverseGeocodingTests {
            public LocatorsForReverseGeocodingTests() {
                var options = new Mock<IOptions<List<ReverseLocatorConfiguration>>>();
                handler = new ReverseGeocodePlan.Handler(options.Object);
            }

            internal IComputationHandler<ReverseGeocodePlan.Computation, IReadOnlyCollection<LocatorProperties>> handler;

            [Fact]
            public async Task Should_return_centerline_geocoder_only() {
                var request = new ReverseGeocodePlan.Computation(1, 2, 3, 4);
                var options = new Mock<IOptions<List<ReverseLocatorConfiguration>>>();
                options.Setup(x => x.Value).Returns(new List<ReverseLocatorConfiguration> {
                    new ReverseLocatorConfiguration {
                        Host = "test",
                        Port = "1",
                        Protocol = "protocol",
                        ServiceName = "service",
                        DisplayName = "correct.answer",
                        ReverseGeocodes = true
                    }, new ReverseLocatorConfiguration {
                        Host = "false",
                        Port = "1",
                        Protocol = "nope",
                        ServiceName = "none",
                        DisplayName = "wrong.answer",
                        ReverseGeocodes = false
                    }
                });

                handler = new ReverseGeocodePlan.Handler(options.Object);

                var result = await handler.Handle(request, CancellationToken.None);

                result.ShouldHaveSingleItem();

                var locator = result.First();
                locator.Url.ShouldBe("protocol://test:1/arcgis/rest/services/Geolocators/service/GeocodeServer/reverseGeocode?f=json&location=1,2&distance=3&outSR=4");
                locator.Name.ShouldBe("correct.answer");
            }
        }

        public class LocatorsForGeocodingTests {
            public LocatorsForGeocodingTests() {
                var options = new Mock<IOptions<List<LocatorConfiguration>>>();
                options.Setup(x => x.Value).Returns(new List<LocatorConfiguration> {
                    new LocatorConfiguration {
                        Host = "road",
                        Port = "1",
                        Protocol = "centerlines",
                        ServiceName = "street_service",
                        DisplayName = "display.street",
                        LocatorType = LocatorType.RoadCenterlines
                    }, new LocatorConfiguration {
                        Host = "address",
                        Port = "1",
                        Protocol = "points",
                        ServiceName = "address_service",
                        DisplayName = "display.address",
                        LocatorType = LocatorType.AddressPoints
                    }
                });

                var logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

                Handler = new GeocodePlan.Handler(options.Object, logger.Object);
            }

            internal IComputationHandler<GeocodePlan.Computation, IReadOnlyCollection<LocatorProperties>> Handler;

            [Fact]
            public async Task Should_create_extra_for_address_reversal() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "2", StreetType.Alley,
                                                        Direction.South, 0, 84114, false, false);
                var address = new AddressWithGrids(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                    Locators = LocatorType.RoadCenterlines,
                    SpatialReference = 26912
                };

                var request = new GeocodePlan.Computation(address, geocodeOptions);
                var result = await Handler.Handle(request, new CancellationToken());

                result.Count.ShouldBe(2);

                result.Count(x => x.Url ==
                                  "centerlines://road:1/arcgis/rest/services/Geolocators/street_service/GeocodeServer/findAddressCandidates?f=json&matchOutOfRange=false&outFields=addr_type,addnum&outSR=26912&Address=1+North+2+Alley+South&City=grid")
                      .ShouldBe(1);

                result.Count(x => x.Url ==
                                  "centerlines://road:1/arcgis/rest/services/Geolocators/street_service/GeocodeServer/findAddressCandidates?f=json&matchOutOfRange=false&outFields=addr_type,addnum&outSR=26912&Address=2+South+Alley+1+North&City=grid")
                      .ShouldBe(1);
            }

            [Fact]
            public async Task Should_return_address_point_geocoder_only() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new AddressWithGrids(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                    Locators = LocatorType.AddressPoints,
                    SpatialReference = 26912
                };

                var request = new GeocodePlan.Computation(address, geocodeOptions);
                var result = await Handler.Handle(request, new CancellationToken());

                result.ShouldHaveSingleItem();

                var locator = result.First();
                locator.Url.ShouldBe("points://address:1/arcgis/rest/services/Geolocators/address_service/GeocodeServer/findAddressCandidates?f=json&matchOutOfRange=false&outFields=addr_type,addnum&outSR=26912&Address=1+North+street+Alley+South&City=grid");
                locator.Name.ShouldBe("display.address");
            }

            [Fact]
            public async Task Should_return_all_geocoders() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new AddressWithGrids(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                    Locators = LocatorType.All,
                    SpatialReference = 26912
                };

                var request = new GeocodePlan.Computation(address, geocodeOptions);
                var result = await Handler.Handle(request, new CancellationToken());

                result.Count.ShouldBe(2);

                geocodeOptions = new SingleGeocodeRequestOptionsContract {
                    Locators = LocatorType.Default,
                    SpatialReference = 26912
                };

                request = new GeocodePlan.Computation(address, geocodeOptions);
                result = await Handler.Handle(request, new CancellationToken());

                result.Count.ShouldBe(2);
            }

            [Fact]
            public async Task Should_return_centerline_geocoder_only() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new AddressWithGrids(parsedAddress) {
                    AddressGrids = new[] { new PlaceGridLink("place", "grid", 0) }
                };

                var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                    Locators = LocatorType.RoadCenterlines,
                    SpatialReference = 26912
                };

                var request = new GeocodePlan.Computation(address, geocodeOptions);
                var result = await Handler.Handle(request, new CancellationToken());

                result.ShouldHaveSingleItem();

                var locator = result.First();
                locator.Url.ShouldBe("centerlines://road:1/arcgis/rest/services/Geolocators/street_service/GeocodeServer/findAddressCandidates?f=json&matchOutOfRange=false&outFields=addr_type,addnum&outSR=26912&Address=1+North+street+Alley+South&City=grid");
                locator.Name.ShouldBe("display.street");
            }

            [Fact]
            public async Task Should_return_empty_when_no_grids() {
                var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                        StreetType.Alley, Direction.South, 0, 84114, false, false);
                var address = new AddressWithGrids(parsedAddress) {
                    AddressGrids = Array.Empty<GridLinkable>()
                };

                var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                    Locators = LocatorType.RoadCenterlines,
                    SpatialReference = 26912
                };

                var request = new GeocodePlan.Computation(address, geocodeOptions);
                var result = await Handler.Handle(request, new CancellationToken());

                result.ShouldBeEmpty();
            }
        }
    }
}
