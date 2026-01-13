using Microsoft.AspNetCore.Http.HttpResults;
using ugrc.api.Features.Converting;
using ugrc.api.Features.Geocoding;
using ugrc.api.Models.ResponseContracts;

namespace api.tests.Features.Geocoding;
public class GeocodeQueryValidationTests {
    private readonly ILogger _logger;
    private readonly Mock<EndpointFilterDelegate> _delegateMock;
    private readonly ApiVersion _version;
    private readonly GeocodeQuery.ValidationFilter _handler;

    public GeocodeQueryValidationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        _delegateMock = new Mock<EndpointFilterDelegate>();

        _version = new ApiVersion(1);
        _handler = new GeocodeQuery.ValidationFilter(new JsonSerializerOptionsFactory(), _version, _logger);
    }

    [Fact]
    public async Task Should_fail_with_no_street_name() {
        var context = TestHelpers.GetEndpointContext(string.Empty, "zip or city", new(), _version);

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("street is a required field. Input was empty.");
    }

    [Fact]
    public async Task Should_fail_with_no_zip_or_city_values() {
        var context = TestHelpers.GetEndpointContext("123 my street", string.Empty, new());

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("zone is a required field. Input was empty.");
    }

    [Fact]
    public async Task Should_fail_with_both_empty() {
        var context = TestHelpers.GetEndpointContext(string.Empty, "     ", null);

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("street is a required field. Input was empty. zone is a required field. Input was empty.");
    }

    [Fact]
    public async Task Should_call_next_with_no_errors() {
        var context = TestHelpers.GetEndpointContext("street", "zone", new());

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;
        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Once);
    }
}
