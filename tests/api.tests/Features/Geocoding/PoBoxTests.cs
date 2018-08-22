using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using api.mapserv.utah.gov.Models.Linkables;
using api.mapserv.utah.gov.Models.RequestOptions;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class PoBoxTests {
        private readonly IDictionary<int, PoBoxAddress> _poBoxes = new Dictionary<int, PoBoxAddress>(1);
        private readonly IDictionary<int, PoBoxAddressCorrection> _exclusions = new Dictionary<int, PoBoxAddressCorrection>(1);
        private readonly IReadOnlyCollection<int> _zipExclusions;
        private readonly PoBoxLocation.Handler _handler;

        public PoBoxTests() {
            _poBoxes.Add(84114, new PoBoxAddress(84114, 1, 1));
            _exclusions.Add(841140001, new PoBoxAddressCorrection(84114, 841140001, 2, 2));
            _zipExclusions = new[] { 84114 };

            var mockCache = new Mock<ILookupCache>();
            mockCache.Setup(x => x.PoBoxes).Returns(_poBoxes);
            mockCache.Setup(x => x.PoBoxExclusions).Returns(_exclusions);
            mockCache.Setup(x => x.PoBoxZipCodesWithExclusions).Returns(_zipExclusions);

            _handler = new PoBoxLocation.Handler(mockCache.Object, new Mock<IMediator>().Object);
        }

        [Fact]
        public async Task Should_return_null_if_zip_not_found() {
            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street", StreetType.Alley, Direction.South, 0, -1, false, false);
            var address = new GeocodeAddress(parsedAddress);

            var geocodeOptions = new GeocodingOptions {
                PoBox = true,
            };

            var request = new PoBoxLocation.Command(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.ShouldBeNull();
        }

        [Fact]
        public async Task Should_return_null_for_address_without_zip_code() {
            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, 0, Direction.North, "street", StreetType.Alley, Direction.South, 0, new Nullable<int>(), false, false);
            var address = new GeocodeAddress(parsedAddress);

            var geocodeOptions = new GeocodingOptions {
                PoBox = true
            };

            var request = new PoBoxLocation.Command(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.ShouldBeNull();
        }

        [Fact]
        public async Task Should_return_candidate_from_poboxes() {
            var pobox = -1;
            var zip = 84114;

            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, pobox, Direction.North, "street", StreetType.Alley, Direction.South, 0, zip, false, false);
            var address = new GeocodeAddress(parsedAddress);
            address.AddressGrids = new[] { new ZipGridLink(84114, "grid", 0) };

            var geocodeOptions = new GeocodingOptions {
                PoBox = true,
                SpatialReference = 26912
            };

            var request = new PoBoxLocation.Command(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Score.ShouldBe(100);
            result.Locator.ShouldBe("Post Office Point");
            result.Location.X.ShouldBe(1);
            result.Location.Y.ShouldBe(1);
            result.AddressGrid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_candidate_from_exclusions() {
            var pobox = 1;
            var zip = 84114;

            var parsedAddress = new CleansedAddress("inputAddress", 1, 0, pobox, Direction.North, "street", StreetType.Alley, Direction.South, 0, zip, false, false);
            var address = new GeocodeAddress(parsedAddress);
            address.AddressGrids = new[] { new ZipGridLink(84114, "grid", 0) };

            var geocodeOptions = new GeocodingOptions {
                PoBox = true,
                SpatialReference = 26912
            };

            var request = new PoBoxLocation.Command(address, geocodeOptions);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Score.ShouldBe(100);
            result.Locator.ShouldBe("Post Office Point Exclusions");
            result.Location.X.ShouldBe(2);
            result.Location.Y.ShouldBe(2);
            result.AddressGrid.ShouldBe("grid");
        }
    }
}
