using System.Net.Http.Json;
using ugrc.api.Features.Geocoding;
using ugrc.api.Models.ArcGis;
using Moq.Protected;

namespace api.tests.Features.Geocoding;
public class GeocodeTests {
    private readonly ILogger _logger;
    private readonly Geocode.Computation _computation;
    public GeocodeTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        _computation = new(new("https://test/locator", "test", 0));
    }

    [Fact]
    public async Task Should_handle_task_canceled_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new TaskCanceledException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeEmpty();
    }
    [Fact]
    public async Task Should_handle_task_http_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new HttpRequestException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_handle_content_reading_errors() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(new StringContent("not json"));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeEmpty();
    }
    [Fact]
    public async Task Should_handle_esri_rest_error() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new LocatorResponse(null, new(500, "missing required parameter", null))));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeEmpty();
    }
    [Fact]
    public async Task Should_handle_esri_no_candidates() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new LocatorResponse(null, null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeEmpty();
    }
    [Fact]
    public async Task Should_handle_remove_weird_pro_candidates() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new LocatorResponse(new List<LocatorCandidate> {
            new LocatorCandidate("remove", new(1,2), 100, new("StreetName", string.Empty)),
            new LocatorCandidate("remove items with no address", new(1,2), 100, new("OtherType", string.Empty)),
        }, null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.ShouldBeEmpty();
    }
    [Fact]
    public async Task Should_include_all_candidates_with_an_address_number_and_intersections() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new LocatorResponse(new List<LocatorCandidate> {
            new LocatorCandidate("remove", new(1,2), 100, new("StreetInt", string.Empty)),
            new LocatorCandidate("remove", new(3,4), 100, new("StreetName", string.Empty)),
            new LocatorCandidate("remove items with no address", new(1,2), 100, new("StreetAddress", "200")),
        }, null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, "https://test/locator");

        var handler = new Geocode.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "arcgis")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://test/locator")),
           ItExpr.IsAny<CancellationToken>()
        );

        result.Count.ShouldBe(2);
    }
}
