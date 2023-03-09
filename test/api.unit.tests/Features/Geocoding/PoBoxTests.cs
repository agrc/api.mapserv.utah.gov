using System.Collections.Generic;
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
    public class PoBoxTests {
        public PoBoxTests() {
            _poBoxes.Add(84114, new PoBoxAddress(84114, 1, 1));
            _exclusions.Add(841140001, new PoBoxAddressCorrection(84114, 841140001, 2, 2));
            IReadOnlyCollection<int> zipExclusions = new[] { 84114 };

            var mockCache = new Mock<IStaticCache>();
            mockCache.Setup(x => x.PoBoxes).Returns(_poBoxes);
            mockCache.Setup(x => x.PoBoxExclusions).Returns(_exclusions);
            mockCache.Setup(x => x.PoBoxZipCodesWithExclusions).Returns(zipExclusions);

            var mock = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

            _handler = new PoBoxLocation.Handler(mockCache.Object, mock.Object);
        }

        private readonly Dictionary<int, PoBoxAddress> _poBoxes = new Dictionary<int, PoBoxAddress>(1);

        private readonly Dictionary<int, PoBoxAddressCorrection> _exclusions =
            new Dictionary<int, PoBoxAddressCorrection>(1);

        private readonly PoBoxLocation.Handler _handler;

        [Fact]
        public async Task Should_return_candidate_from_exclusions() {
            const int pobox = 1;
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

            var request = new PoBoxLocation.Computation(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Score.ShouldBe(100);
            result.Locator.ShouldBe("Post Office Point Exclusions");
            result.Location.X.ShouldBe(2);
            result.Location.Y.ShouldBe(2);
            result.AddressGrid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_candidate_from_poboxes() {
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

            var request = new PoBoxLocation.Computation(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Score.ShouldBe(100);
            result.Locator.ShouldBe("Post Office Point");
            result.Location.X.ShouldBe(1);
            result.Location.Y.ShouldBe(1);
            result.AddressGrid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_null_for_address_without_zip_code() {
            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street",
                                                    StreetType.Alley, Direction.South, 0, new int?(), false, false);
            var address = new AddressWithGrids(parsedAddress);

            var geocodeOptions = new SingleGeocodeRequestOptionsContract {
                PoBox = true
            };

            var request = new PoBoxLocation.Computation(address, geocodeOptions);
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

            var request = new PoBoxLocation.Computation(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.ShouldBeNull();
        }
    }
}
