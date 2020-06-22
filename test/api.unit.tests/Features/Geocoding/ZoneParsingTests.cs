using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class ZoneParsingTests {
        public ZoneParsingTests() {
            var regex = new RegexCache(new Abbreviations());
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.Send(It.IsAny<AddressSystemFromPlace.Command>(),
                                       It.IsAny<CancellationToken>()))
                    .Returns((AddressSystemFromPlace.Command g, CancellationToken t) => {
                        if (g.CityKey == "alta") {
                            return Task.FromResult(new[] {new PlaceGridLink("alta", "grid", 1)} as
                                                       IReadOnlyCollection<GridLinkable>);
                        }

                        return Task.FromResult(Array.Empty<GridLinkable>() as IReadOnlyCollection<GridLinkable>);
                    });

            var mock = new Mock<ILogger>();
            mock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(new Mock<ILogger>().Object);

            _handler = new ZoneParsing.Handler(regex, mediator.Object, mock.Object);
        }

        private readonly ZoneParsing.Handler _handler;

        [Theory]
        [InlineData("123456789")]
        [InlineData("12345-6789")]
        public async Task Should_parse_zip_parts(string input) {
            var address = new AddressWithGrids(new CleansedAddress());
            var request = new ZoneParsing.Command(input, address);

            var result = await _handler.Handle(request, new CancellationToken());

            result.Zip5.ShouldBe(12345);
            result.Zip4.ShouldBe(6789);
        }

        [Theory]
        [InlineData("City of Alta.")]
        [InlineData("Town of     Alta")]
        [InlineData("Alta ")]
        public async Task Should_find_grid_from_place(string input) {
            var address = new AddressWithGrids(new CleansedAddress());
            var request = new ZoneParsing.Command(input, address);

            var result = await _handler.Handle(request, new CancellationToken());

            result.AddressGrids.ShouldHaveSingleItem();
            result.AddressGrids.First().Grid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_empty_grid_if_zone_not_found() {
            var address = new AddressWithGrids(new CleansedAddress());
            var request = new ZoneParsing.Command("123eastbumble", address);

            var result = await _handler.Handle(request, new CancellationToken());

            result.AddressGrids.ShouldBeEmpty();
        }
    }
}
