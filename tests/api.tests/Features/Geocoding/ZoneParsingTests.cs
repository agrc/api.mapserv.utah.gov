using Shouldly;
using Moq;
using Xunit;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using MediatR;
using System.Collections.Generic;
using api.mapserv.utah.gov.Models.Constants;
using System;
using System.Linq;
using api.mapserv.utah.gov.Models.Linkables;
using System.Collections.ObjectModel;

namespace api.tests.Features.Geocoding {
    public class ZoneParsingTests {
        private readonly ZoneParsing.Handler _handler;
        public ZoneParsingTests() {
            var abbr = new Mock<IAbbreviations>();
            abbr.Setup(x => x.StreetTypeAbbreviations).Returns(new Dictionary<StreetType, string>());
            abbr.Setup(x => x.UnitAbbreviations).Returns(new List<Tuple<string, string, bool>>());

            var regex = new RegexCache(abbr.Object);
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.Send(It.IsAny<DoubleAvenuesException.Command>(),
                                       It.IsAny<CancellationToken>()))
                    .Returns((DoubleAvenuesException.Command g, CancellationToken t) => Task.FromResult(g.Address));
            mediator.Setup(x => x.Send(It.IsAny<AddressSystemFromPlace.Command>(),
                                       It.IsAny<CancellationToken>()))
                    .Returns((AddressSystemFromPlace.Command g, CancellationToken t) => {
                        if (g.CityKey == "alta") {
                            return Task.FromResult(new[] { new PlaceGridLink("alta", "grid", 1) } as IReadOnlyCollection<GridLinkable>);
                        } else {
                            return Task.FromResult(Array.Empty<GridLinkable>() as IReadOnlyCollection<GridLinkable>);
                        }
                    });


            _handler = new ZoneParsing.Handler(regex, mediator.Object);
        }

        [Theory]
        [InlineData("123456789")]
        [InlineData("12345-6789")]
        public async Task Should_parse_zip_parts(string input) {
            var address = new GeocodeAddress(new CleansedAddress());
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
            var address = new GeocodeAddress(new CleansedAddress());
            var request = new ZoneParsing.Command(input, address);

            var result = await _handler.Handle(request, new CancellationToken());

            result.AddressGrids.ShouldHaveSingleItem();
            result.AddressGrids.First().Grid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_empty_grid_if_zone_not_found() {
            var address = new GeocodeAddress(new CleansedAddress());
            var request = new ZoneParsing.Command("123eastbumble", address);

            var result = await _handler.Handle(request, new CancellationToken());

            result.AddressGrids.ShouldBeEmpty();
        }
    }
}
