using System.Net;
using System.Net.Http.Json;
using AGRC.api.Features.Milepost;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq.Protected;

namespace api.tests.Features.Geocoding;
public class RouteMilepostQueryTests {
    private readonly ILogger _logger;
    private readonly ApiVersion _version;

    public RouteMilepostQueryTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        _version = new ApiVersion(1);
    }
    [Fact]
    public async Task Should_handle_task_canceled_exceptions() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ThrowsAsync(new TaskCanceledException())
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        result.Value.Message.ShouldBe("There was a problem handling your request.");

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_task_http_exceptions() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ThrowsAsync(new HttpRequestException())
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        result.Value.Message.ShouldBe("There was a problem handling your request.");

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_content_reading_errors() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
               Content = new StringContent("not json")
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        result.Value.Message.ShouldBe("There was a problem handling your request.");

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_error_response_content() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
               Content = JsonContent.Create(new MeasureToGeometry.ResponseContract(null, null, new(400, "missing required parameter", null)))
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        result.Value.Message.ShouldBe("Your request was invalid. Check your inputs.");

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_returns_404_when_there_are_no_locations() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
               Content = JsonContent.Create(new MeasureToGeometry.ResponseContract(null, null, null))
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        result.Value.Message.ShouldBe("No milepost was found within your buffer radius.");

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_returns_404_when_status_is_not_ok() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
               Content = JsonContent.Create(new MeasureToGeometry.ResponseContract(new[] {
                new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingCannotFindRoute, null, null, GeometryType.esriGeometryPoint, null),
               }, null, null))
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        result.Value.Message.ShouldBe("esriLocatingCannotFindRoute");

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_returns_first_location() {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
               Content = JsonContent.Create(new MeasureToGeometry.ResponseContract(new[] {
                new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingOK, null, "0015PM", GeometryType.esriGeometryPoint, new MeasureToGeometry.MeasurePoint(1, 2, 80)),
                new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingOK, null, "0015PM", GeometryType.esriGeometryPoint, new MeasureToGeometry.MeasurePoint(3, 4, 5))
               }, null, null))
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("https://maps.udot.utah.gov/"),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == "udot"))).Returns(httpClient);

        var query = new RouteMilepostQuery.Query("15", "80", new(), new(), _version);
        var handler = new RouteMilepostQuery.Handler(httpClientFactory.Object, _logger);

        var result = await handler.Handle(query, CancellationToken.None) as JsonHttpResult<ApiResponseContract>;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.Value.Result.ShouldBeAssignableTo<RouteMilepostResponseContract>();

        var response = result.Value.Result as RouteMilepostResponseContract;
        response.MatchRoute.ShouldBe("Route 15P, Milepost 80");
        response.Source.ShouldBe("UDOT Roads and Highways");
        response.Location.X.ShouldBe(1);
        response.Location.Y.ShouldBe(2);

        var expectedUri = new Uri("""https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/measureToGeometry?f=json&locations=%5B{"routeId"%3A"0015PM","measure"%3A"80"}%5D&outSR=26912""");

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
}
