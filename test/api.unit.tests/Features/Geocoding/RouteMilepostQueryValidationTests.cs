using AGRC.api.Features.Converting;
using AGRC.api.Features.Milepost;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.tests.Features.Geocoding;
public class RouteMilepostQueryValidationTests {
    private readonly ILogger _logger;
    private readonly Mock<EndpointFilterDelegate> _delegateMock;
    private readonly ApiVersion _version;
    private readonly RouteMilepostQuery.ValidationFilter _handler;

    public RouteMilepostQueryValidationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        _delegateMock = new Mock<EndpointFilterDelegate>();

        _version = new ApiVersion(1);
        _handler = new RouteMilepostQuery.ValidationFilter(new JsonSerializerOptionsFactory(), _version, _logger);
    }

    [Fact]
    public async Task Should_fail_with_no_route_name() {
        var context = HttpContextHelpers.GetEndpointContext(string.Empty, "89", new(), _version);

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("route is a required field. Input was empty.");
    }
    [Fact]
    public async Task Should_fail_with_no_milepost_values() {
        var context = HttpContextHelpers.GetEndpointContext("15", string.Empty, new());

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("milepost is a required field. Input was empty.");
    }
    [Fact]
    public async Task Should_fail_with_negative_milepost_values() {
        var context = HttpContextHelpers.GetEndpointContext("15", "-80", new());

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("milepost is a positive value. Input was negative.");
    }
    [Fact]
    public async Task Should_fail_with_invalid_milepost_values() {
        var context = HttpContextHelpers.GetEndpointContext("15", "a", new());

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("milepost is a number value. Input was not a number.");
    }
    [Fact]
    public async Task Should_fail_with_both_empty() {
        var context = HttpContextHelpers.GetEndpointContext(string.Empty, "     ", null);

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;

        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBeAssignableTo<ApiResponseContract>();

        var response = result.Value;
        response.Status.ShouldBe(400);
        response.Message.ShouldBe("route is a required field. Input was empty. milepost is a required field. Input was empty.");
    }

    [Fact]
    public async Task Should_call_next_with_no_errors() {
        var context = HttpContextHelpers.GetEndpointContext("15", "300", new());

        var result = await _handler.InvokeAsync(context, _delegateMock.Object) as JsonHttpResult<ApiResponseContract>;
        _delegateMock.Verify(x => x(It.IsAny<EndpointFilterInvocationContext>()), Times.Once);
    }
}
