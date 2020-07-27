using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.Constants;
using AGRC.api.Models.Linkables;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class DoubleAveExceptionTests {
        private readonly DoubleAvenuesException.Decorator _decorator;
        private readonly ZoneParsing.Handler _computationHandler;

        public DoubleAveExceptionTests() {
            var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

            var mediator = new Mock<IComputeMediator>();

            mediator.Setup(x => x.Handle(It.IsAny<AddressSystemFromPlace.Computation>(), It.IsAny<CancellationToken>()))
                    .Returns((AddressSystemFromPlace.Computation g, CancellationToken t) => {
                        if (g?.CityKey == "slc") {
                            return Task.FromResult(new[] {new PlaceGridLink("slc", "salt lake city", 1)} as
                                                       IReadOnlyCollection<GridLinkable>);
                        }

                        return Task.FromResult(Array.Empty<GridLinkable>() as IReadOnlyCollection<GridLinkable>);
                    });
            mediator.Setup(x => x.Handle(It.IsAny<AddressSystemFromZipCode.Computation>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(Array.Empty<GridLinkable>() as IReadOnlyCollection<GridLinkable>));

            var regexCache = new RegexCache(new Abbreviations());
            _computationHandler = new ZoneParsing.Handler(regexCache, mediator.Object, mockLogger.Object);
            _decorator = new DoubleAvenuesException.Decorator(_computationHandler, regexCache, mockLogger.Object);
        }

        [Theory]
        [InlineData("7th", 84047, Direction.West)]
        [InlineData("7 th", 84047, Direction.West)]
        [InlineData("seventh", 84047, Direction.West)]
        [InlineData("7", 84047, Direction.West)]
        [InlineData("seventh heaven", 84047, Direction.None)]
        [InlineData("7", 11111, Direction.None)]
        [InlineData("7th", 11111, Direction.None)]
        [InlineData("7th", 0, Direction.None)]
        public async Task Should_add_west_to_midvale_avenue_if_not_supplied_for_zip(
            string streetname, int zipcode, Direction direction) {
            var address = new AddressWithGrids(new CleansedAddress("", 0, 0, 0, Direction.None, streetname,
                                                                 StreetType.Avenue, Direction.None, 0, zipcode, false,
                                                                 false));

            var request = new ZoneParsing.Computation(zipcode.ToString(), address);
            var result = await _decorator.Handle(request, new CancellationToken());

            result.PrefixDirection.ShouldBe(direction);
        }

        [Theory]
        [InlineData("7th", "MIDVale", Direction.West)]
        [InlineData("7 th", "midvale", Direction.West)]
        [InlineData("seventh", " midvale ", Direction.West)]
        [InlineData("7", "  midvale", Direction.West)]
        [InlineData("7", "not problem area", Direction.None)]
        [InlineData("7th", "not problem area", Direction.None)]
        [InlineData("7th", null, Direction.None)]
        public async Task Should_add_west_to_midvale_avenue_if_not_supplied_for_city(
            string streetname, string city, Direction direction) {
            var address = new AddressWithGrids(new CleansedAddress("", 0, 0, 0, Direction.None, streetname,
                                                                 StreetType.Avenue, Direction.None, 0, null, false,
                                                                 false));

            var request = new ZoneParsing.Computation(city, address);
            var result = await _decorator.Handle(request, new CancellationToken());

            result.PrefixDirection.ShouldBe(direction);
        }

        [Theory]
        [InlineData("7th", "slc", Direction.East)]
        [InlineData("7 th", "slc", Direction.East)]
        [InlineData("seventh", " slc ", Direction.East)]
        [InlineData("7", "  slc", Direction.East)]
        public async Task Should_add_east_to_slc_avenue_if_not_supplied_for_city(
            string streetname, string city, Direction direction) {
            var address = new AddressWithGrids(new CleansedAddress("", 0, 0, 0, Direction.None, streetname,
                                                                 StreetType.Avenue, Direction.None, 0, null, false,
                                                                 false));

            var request = new ZoneParsing.Computation(city, address);
            var result = await _decorator.Handle(request, new CancellationToken());

            result.PrefixDirection.ShouldBe(direction);
        }
    }
}
