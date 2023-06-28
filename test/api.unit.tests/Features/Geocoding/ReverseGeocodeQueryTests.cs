using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;

namespace api.tests.Features.Geocoding;

public class ReverseGeocodeQueryTests {
    private readonly ILogger _logger;
    private readonly ReverseGeocodeQuery.Query _query;
    public ReverseGeocodeQueryTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        _query = new(1, 2, new());
    }
    [Fact]
    public async Task Should_return_404_when_no_plan_is_created() {
        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocodePlan.Computation __, CancellationToken _) =>
               Array.Empty<LocatorProperties>()
            );

        var handler = new ReverseGeocodeQuery.Handler(mediator.Object, _logger);

        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;
        result.Message.ShouldBe("No address candidates found within 5 meters of 1,2.");
        result.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
    [Fact]
    public async Task Should_return_404_when_no_address_is_found() {
        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocodePlan.Computation __, CancellationToken _) =>
               new List<LocatorProperties>() { new LocatorProperties("https://url.com", "name", 0) }
            );
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocode.Computation __, CancellationToken _) =>
              null
            );

        var handler = new ReverseGeocodeQuery.Handler(mediator.Object, _logger);

        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;
        result.Message.ShouldBe("No address candidates found within 5 meters of 1,2.");
        result.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Should_return_404_when_no_address_is_empty() {
        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocodePlan.Computation __, CancellationToken _) =>
               new List<LocatorProperties>() { new LocatorProperties("https://url.com", "name", 0) }
            );
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocode.Computation __, CancellationToken _) =>
              new(new(string.Empty, "city", "type"), new(1, 2), null)
            );

        var handler = new ReverseGeocodeQuery.Handler(mediator.Object, _logger);

        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;
        result.Message.ShouldBe("No address candidates found within 5 meters of 1,2.");
        result.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
    [Fact]
    public async Task Should_return_response_object() {
        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocodePlan.Computation __, CancellationToken _) =>
               new List<LocatorProperties>() { new LocatorProperties("https://url.com", "name", 0) }
            );
        mediator.Setup(x => x.Handle(It.IsAny<ReverseGeocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReverseGeocode.Computation __, CancellationToken _) =>
              new(new("address", "city", "type"), new(1, 2), null)
            );

        var handler = new ReverseGeocodeQuery.Handler(mediator.Object, _logger);

        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;
        result.Status.ShouldBe(StatusCodes.Status200OK);

        result.Result.ShouldBeOfType<ReverseGeocodeResponseContract>();
        var contract = result.Result as ReverseGeocodeResponseContract;
        contract.Address.Street.ShouldBe("address");
        contract.Address.AddressType.ShouldBe("type");
        contract.Address.AddressSystem.ShouldBe("city");
        contract.MatchPoint.X.ShouldBe(1);
        contract.MatchPoint.Y.ShouldBe(2);
    }
}
