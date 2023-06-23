using System.Net.Http.Json;
using AGRC.api.Features.Milepost;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using Moq.Protected;

namespace api.tests.Features.Geocoding;
public class RouteMilepostQueryTests {
    private readonly ILogger _logger;
    private readonly ApiVersion _version;
    private readonly Uri _expectedUri = new("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");
    private readonly RouteMilepostQuery.Query _query;


    public RouteMilepostQueryTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        _version = new ApiVersion(1);
        _query = new("15", "80", new(), _version);
    }
    [Fact]
    public async Task Should_handle_task_canceled_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new TaskCanceledException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        result.Message.ShouldBe("The request was canceled.");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_task_http_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new HttpRequestException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");

        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        result.Message.ShouldBe("There was a problem handling your request.");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_content_reading_errors() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(new StringContent("not json"));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");

        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        result.Message.ShouldBe("There was an unexpected response from UDOT.");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_error_response_content() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new MeasureToGeometry.ResponseContract(null, null, new(400, "missing required parameter", null)))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");

        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status400BadRequest);
        result.Message.ShouldBe("Your request was invalid. Check that your coordinates and spatial reference match.");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_returns_404_when_there_are_no_locations() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new MeasureToGeometry.ResponseContract(null, null, null))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");

        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status404NotFound);
        result.Message.ShouldBe("No milepost was found within your buffer radius.");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_returns_404_when_status_is_not_ok() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new MeasureToGeometry.ResponseContract(new[] {
                new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingCannotFindRoute, null, null, GeometryType.esriGeometryPoint, null),
            }, null, null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");

        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status404NotFound);
        result.Message.ShouldBe("esriLocatingCannotFindRoute");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_returns_first_location() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new MeasureToGeometry.ResponseContract(new[] {
                new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingOK, null, "0015PM", GeometryType.esriGeometryPoint, new MeasureToGeometry.MeasurePoint(1, 2, 80)),
                new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingOK, null, "0015PM", GeometryType.esriGeometryPoint, new MeasureToGeometry.MeasurePoint(3, 4, 5))
               }, null, null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://maps.udot.utah.gov/");

        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status200OK);
        result.Result.ShouldBeAssignableTo<RouteMilepostResponseContract>();

        var response = result.Result as RouteMilepostResponseContract;
        response.MatchRoute.ShouldBe("Route 15P, Milepost 80");
        response.Source.ShouldBe("UDOT Roads and Highways");
        response.Location.X.ShouldBe(1);
        response.Location.Y.ShouldBe(2);

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
}
