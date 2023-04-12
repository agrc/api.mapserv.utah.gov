using AGRC.api.Features.Geocoding;
using AGRC.api.Models.ResponseContracts;

namespace api.tests.Features.Geocoding;
public class GeocodeQueryValidationTests {
    private readonly ILogger _logger;
    private readonly Mock<RequestHandlerDelegate<ObjectResult>> delegateMock;
    public GeocodeQueryValidationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        delegateMock = new Mock<RequestHandlerDelegate<ObjectResult>>();
        delegateMock.Setup(x => x());
    }

    [Fact]
    public async Task Should_fail_with_no_street_name() {
        var query = new GeocodeQuery.Query(string.Empty, "zip or city", new SingleGeocodeRequestOptionsContract());

        var handler = new GeocodeQuery.ValidationBehavior<GeocodeQuery.Query, ObjectResult>(_logger);
        var result = await handler.Handle(query, delegateMock.Object, CancellationToken.None);

        delegateMock.Verify(x => x(), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract<SingleGeocodeResponseContract>>();

        var response = result.Value as ApiResponseContract<SingleGeocodeResponseContract>;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("Street is empty.");
    }

    [Fact]
    public async Task Should_fail_with_no_zip_or_city_values() {
        var query = new GeocodeQuery.Query("123 my street", string.Empty, new SingleGeocodeRequestOptionsContract());

        var handler = new GeocodeQuery.ValidationBehavior<GeocodeQuery.Query, ObjectResult>(_logger);
        var result = await handler.Handle(query, delegateMock.Object, CancellationToken.None);

        delegateMock.Verify(x => x(), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract<SingleGeocodeResponseContract>>();

        var response = result.Value as ApiResponseContract<SingleGeocodeResponseContract>;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("Zip code or city name is empty");
    }

    [Fact]
    public async Task Should_fail_with_both_empty() {
        var query = new GeocodeQuery.Query(string.Empty, "     ", null);

        var handler = new GeocodeQuery.ValidationBehavior<GeocodeQuery.Query, ObjectResult>(_logger);
        var result = await handler.Handle(query, delegateMock.Object, CancellationToken.None);

        delegateMock.Verify(x => x(), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract<SingleGeocodeResponseContract>>();

        var response = result.Value as ApiResponseContract<SingleGeocodeResponseContract>;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("Street is empty.Zip code or city name is empty");
    }

    [Fact]
    public async Task Should_call_next_with_no_errors() {
        var query = new GeocodeQuery.Query("street", "zone", new SingleGeocodeRequestOptionsContract());

        var handler = new GeocodeQuery.ValidationBehavior<GeocodeQuery.Query, ObjectResult>(_logger);
        var result = await handler.Handle(query, delegateMock.Object, CancellationToken.None);

        delegateMock.Verify(x => x(), Times.Once);
    }
}
