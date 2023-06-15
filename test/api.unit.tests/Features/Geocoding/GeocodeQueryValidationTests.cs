using AGRC.api.Features.Geocoding;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.tests.Features.Geocoding;
public class GeocodeQueryValidationTests {
    private readonly ILogger _logger;
    private readonly Mock<RequestHandlerDelegate<IApiResponse>> delegateMock;
    public GeocodeQueryValidationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        delegateMock = new Mock<RequestHandlerDelegate<IApiResponse>>();
        delegateMock.Setup(x => x());
    }

    // [Fact]
    // public async Task Should_fail_with_no_street_name() {
    //     var query = new GeocodeQuery.Query(string.Empty, "zip or city", new());

    //     var handler = new GeocodeQuery.ValidationFilter(_logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<SingleGeocodeResponseContract>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<SingleGeocodeResponseContract>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("Street is empty.");
    // }

    // [Fact]
    // public async Task Should_fail_with_no_zip_or_city_values() {
    //     var query = new GeocodeQuery.Query("123 my street", string.Empty, new());

    //     var handler = new GeocodeQuery.ValidationFilter(_logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<SingleGeocodeResponseContract>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<SingleGeocodeResponseContract>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("Zip code or city name is empty");
    // }

    // [Fact]
    // public async Task Should_fail_with_both_empty() {
    //     var query = new GeocodeQuery.Query(string.Empty, "     ", null);

    //     var handler = new GeocodeQuery.ValidationFilter(_logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<SingleGeocodeResponseContract>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<SingleGeocodeResponseContract>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("Street is empty.Zip code or city name is empty");
    // }

    // [Fact]
    // public async Task Should_call_next_with_no_errors() {
    //     var query = new GeocodeQuery.Query("street", "zone", new());

    //     var handler = new GeocodeQuery.ValidationFilter(_logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None);

    //     delegateMock.Verify(x => x(), Times.Once);
    // }
}
