using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class DoubleAveExceptionTests {
        private readonly IPipelineBehavior<ZoneParsing.Command, AddressWithGrids> _handler;
        private readonly ZoneParsing.Handler _requestHandler;

        public DoubleAveExceptionTests() {
            var mock = new Mock<ILogger>();
            mock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(new Mock<ILogger>().Object);

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.Send(It.IsAny<AddressSystemFromPlace.Command>(), It.IsAny<CancellationToken>()))
                    .Returns((AddressSystemFromPlace.Command g, CancellationToken t) => {
                        if (g?.CityKey == "slc") {
                            return Task.FromResult(new[] {new PlaceGridLink("slc", "salt lake city", 1)} as
                                                       IReadOnlyCollection<GridLinkable>);
                        }

                        return Task.FromResult(Array.Empty<GridLinkable>() as IReadOnlyCollection<GridLinkable>);
                    });
            mediator.Setup(x => x.Send(It.IsAny<AddressSystemFromZipCode.Command>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(Array.Empty<GridLinkable>() as IReadOnlyCollection<GridLinkable>));

            var regex = new RegexCache(new Abbreviations());
            _handler =
                new DoubleAvenuesException.DoubleAvenueExceptionPipeline<ZoneParsing.Command, AddressWithGrids>(regex, mock.Object);
            _requestHandler = new ZoneParsing.Handler(regex, mediator.Object, mock.Object);
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

            var request = new ZoneParsing.Command(zipcode.ToString(), address);
            var result = await _handler.Handle(request, new CancellationToken(),
                                               () => _requestHandler.Handle(request, CancellationToken.None));

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

            var request = new ZoneParsing.Command(city, address);
            var result = await _handler.Handle(request, new CancellationToken(),
                                               () => _requestHandler.Handle(request, CancellationToken.None));

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

            var request = new ZoneParsing.Command(city, address);
            var result = await _handler.Handle(request, new CancellationToken(),
                                               () => _requestHandler.Handle(request, CancellationToken.None));

            result.PrefixDirection.ShouldBe(direction);
        }
    }
}
