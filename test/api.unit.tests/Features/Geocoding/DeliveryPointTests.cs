using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Models.Constants;
using AGRC.api.Models.Linkables;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class DeliveryPointTests {
        public DeliveryPointTests() {
            _deliveryPoints.Add("84114",
                                new GridLinkable[] { new UspsDeliveryPointLink(84114, "grid", "place", 1, 1) }.ToList());

            var mockCache = new Mock<IStaticCache>();
            mockCache.Setup(x => x.UspsDeliveryPoints).Returns(_deliveryPoints);

            var mock = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

            _handler = new UspsDeliveryPointLocation.Handler(mockCache.Object, mock.Object);
        }

        private readonly Dictionary<string, List<GridLinkable>> _deliveryPoints = new(1);
        private readonly UspsDeliveryPointLocation.Handler _handler;

        [Fact]
        public async Task Should_return_candidate_from_cache() {
            const int pobox = -1;
            const int zip = 84114;

            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, pobox, Direction.North, "street",
                                                    StreetType.Alley, Direction.South, 0, zip, false, false);
            var address = new AddressWithGrids(parsedAddress) {
                AddressGrids = new[] { new ZipGridLink(84114, "grid", 0) }
            };

            var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                PoBox = true,
                SpatialReference = 26912
            };

            var request = new UspsDeliveryPointLocation.Computation(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Score.ShouldBe(100);
            result.Locator.ShouldBe("USPS Delivery Points");
            result.Location.X.ShouldBe(1);
            result.Location.Y.ShouldBe(1);
            result.AddressGrid.ShouldBe("GRID");
        }

        [Fact]
        public async Task Should_return_null_for_address_without_zip_code() {
            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                    StreetType.Alley, Direction.South, 0, new int?(), false, false);
            var address = new AddressWithGrids(parsedAddress);

            var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                PoBox = true
            };

            var request = new UspsDeliveryPointLocation.Computation(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.ShouldBeNull();
        }

        [Fact]
        public async Task Should_return_null_if_zip_not_found() {
            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                    StreetType.Alley, Direction.South, 0, -1, false, false);
            var address = new AddressWithGrids(parsedAddress);

            var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                PoBox = true
            };

            var request = new UspsDeliveryPointLocation.Computation(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.ShouldBeNull();
        }
    }
}
