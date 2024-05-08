using ugrc.api.Cache;
using ugrc.api.Features.Geocoding;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Constants;
using ugrc.api.Models.Linkables;

namespace api.tests.Features.Geocoding;
public class DoubleAveExceptionTests {
    private readonly DoubleAvenuesException.Decorator _decorator;
    private readonly ZoneParsing.Handler _computationHandler;

    public DoubleAveExceptionTests() {
        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        var mediator = new Mock<IComputeMediator>();

        mediator.Setup(x => x.Handle(It.IsAny<AddressSystemFromPlace.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AddressSystemFromPlace.Computation g, CancellationToken _) => {
                    if (g?._cityKey == "slc") {
                        return [new PlaceGridLink("slc", "salt lake city", 1)];
                    }

                    return [];
                });
        mediator.Setup(x => x.Handle(It.IsAny<AddressSystemFromZipCode.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

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
        string streetName, int zipCode, Direction direction) {
        var inputAddress = string.Empty;
        int? houseNumber = null;
        const Direction prefixDirection = Direction.None;
        const StreetType streetType = StreetType.Avenue;
        const Direction suffixDirection = Direction.None;
        const int zip5 = 0;
        const int poBox = 0;
        const bool isPoBox = false;
        const bool isHighway = false;

        var address = new Address(inputAddress, houseNumber, prefixDirection, streetName, streetType,
                                suffixDirection, zipCode, zip5, null, poBox, isPoBox, isHighway);

        var request = new ZoneParsing.Computation(zipCode.ToString(), address);
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
        string streetName, string city, Direction direction) {
        var inputAddress = string.Empty;
        int? houseNumber = null;
        const Direction prefixDirection = Direction.None;
        const StreetType streetType = StreetType.Avenue;
        const Direction suffixDirection = Direction.None;
        const int zip4 = 0;
        const int zip5 = 0;
        const int poBox = 0;
        const bool isPoBox = false;
        const bool isHighway = false;

        var address = new Address(inputAddress, houseNumber, prefixDirection, streetName, streetType,
                                suffixDirection, zip4, zip5, null, poBox, isPoBox, isHighway);

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
        string streetName, string city, Direction direction) {
        var inputAddress = string.Empty;
        int? houseNumber = null;
        const Direction prefixDirection = Direction.None;
        const StreetType streetType = StreetType.Avenue;
        const Direction suffixDirection = Direction.None;
        const int zip4 = 0;
        const int zip5 = 0;
        const int poBox = 0;
        const bool isPoBox = false;
        const bool isHighway = false;

        var address = new Address(inputAddress, houseNumber, prefixDirection, streetName, streetType,
                                suffixDirection, zip4, zip5, null, poBox, isPoBox, isHighway);

        var request = new ZoneParsing.Computation(city, address);
        var result = await _decorator.Handle(request, new CancellationToken());

        result.PrefixDirection.ShouldBe(direction);
    }
}
