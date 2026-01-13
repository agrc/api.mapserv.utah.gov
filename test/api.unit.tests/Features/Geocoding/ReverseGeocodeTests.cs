using System.Net.Http.Json;
using Moq.Protected;
using ugrc.api.Features.Geocoding;
using ugrc.api.Models.ArcGis;

namespace api.tests.Features.Geocoding;

public class ReverseGeocodeTests {
    private readonly ILogger _logger;
    private readonly ReverseGeocode.Computation _computation;
    public ReverseGeocodeTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        _computation = new(new("https://test/locator", "test", 0));
    }
    [Fact]
    public async Task Should_handle_task_canceled_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new TaskCanceledException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new ReverseGeocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeNull();
    }
    [Fact]
    public async Task Should_handle_task_http_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new HttpRequestException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new ReverseGeocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeNull();
    }
    [Fact]
    public async Task Should_handle_content_reading_errors() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(new StringContent("not json"));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new ReverseGeocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeNull();
    }
    [Fact]
    public async Task Should_handle_esri_rest_error() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new ReverseGeocodeRestResponse(null, null, new(500, "missing required parameter", null))));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new ReverseGeocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeNull();
    }
    [Fact]
    public async Task Should_returns_response() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new ReverseGeocodeRestResponse(new("address", "city", "type"), new(1, 2), null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new ReverseGeocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        result.Address.Address.ShouldBe("address");
        result.Address.City.ShouldBe("city");
        result.Address.Addr_type.ShouldBe("type");
        result.Location.X.ShouldBe(1);
        result.Location.Y.ShouldBe(2);

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );
    }
}
