using ugrc.api.Cache;
using ugrc.api.Features.Geocoding;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;

namespace api.tests.Features.Geocoding;
public class ZoneParsingTests {
    public ZoneParsingTests() {
        var regex = new RegexCache(new Abbreviations());
        var mediator = new Mock<IComputeMediator>();

        mediator.Setup(x => x.Handle(It.IsAny<AddressSystemFromPlace.Computation>(),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync((AddressSystemFromPlace.Computation g, CancellationToken _) => {
                    if (g._cityKey == "alta") {
                        return [new PlaceGridLink("alta", "grid", 1)];
                    }

                    return [];
                });

        var mock = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        _handler = new ZoneParsing.Handler(regex, mediator.Object, mock.Object);
    }

    private readonly ZoneParsing.Handler _handler;

    [Theory]
    [InlineData("123456789")]
    [InlineData("12345-6789")]
    public async Task Should_parse_zip_parts(string input) {
        var address = AddressHelper.CreateEmptyAddress();
        var request = new ZoneParsing.Computation(input, address);

        var result = await _handler.Handle(request, new CancellationToken());

        result.Zip5.ShouldBe(12345);
        result.Zip4.ShouldBe(6789);
    }

    [Theory]
    [InlineData("City of Alta.")]
    [InlineData("Town of     Alta")]
    [InlineData("Alta ")]
    [InlineData("Alta City")]
    public async Task Should_find_grid_from_place(string input) {
        var address = AddressHelper.CreateEmptyAddress();
        var request = new ZoneParsing.Computation(input, address);

        var result = await _handler.Handle(request, new CancellationToken());

        result.AddressGrids.ShouldHaveSingleItem();
        result.AddressGrids.First().Grid.ShouldBe("grid");
    }

    [Fact]
    public async Task Should_return_empty_grid_if_zone_not_found() {
        var address = AddressHelper.CreateEmptyAddress();
        var request = new ZoneParsing.Computation("123eastbumble", address);

        var result = await _handler.Handle(request, new CancellationToken());

        result.AddressGrids.ShouldBeEmpty();
    }
}
