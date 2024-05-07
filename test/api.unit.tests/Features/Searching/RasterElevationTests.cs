using System.Net.Http.Json;
using ugrc.api.Features.Searching;
using ugrc.api.Models.ArcGis;
using Moq.Protected;

namespace api.tests.Features.Searching;

public class RasterElevationTests {
    private readonly ILogger _logger;
    private readonly Uri _expectedUri;
    private readonly string _baseUrl;
    private readonly RasterElevation.Computation _computation;
    public RasterElevationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        _baseUrl = "https://elevation.nationalmap.gov/arcgis/rest/services/3DEPElevation/ImageServer/identify";
        _expectedUri = new Uri("""https://elevation.nationalmap.gov/arcgis/rest/services/3DEPElevation/ImageServer/identify?geometry={"x":1,"y":2,"spatialReference":{"wkid":3}}&geometryType=esriGeometryPoint&returnGeometry=false&returnCatalogItems=false&f=json""");
        _computation = new RasterElevation.Computation("feet", new(new()) {
            Point = new(1, 2, new(3, null))
        });
    }

    [Fact]
    public async Task Should_validate_input() {
        var computation = new RasterElevation.Computation("feet", new(new()));

        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new TaskCanceledException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("arcgis", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);

        await Should.ThrowAsync<ArgumentException>(async () => await handler.Handle(computation, CancellationToken.None));
    }
    [Fact]
    public async Task Should_handle_task_canceled_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new TaskCanceledException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("national-map", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);
        await Should.ThrowAsync<TaskCanceledException>(async () => await handler.Handle(_computation, CancellationToken.None));

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "national-map")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_task_http_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new HttpRequestException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("national-map", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);
        await Should.ThrowAsync<HttpRequestException>(async () => await handler.Handle(_computation, CancellationToken.None));

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "national-map")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_content_reading_errors() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(new StringContent("not json"));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("national-map", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);
        await Should.ThrowAsync<Exception>(async () => await handler.Handle(_computation, CancellationToken.None));

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "national-map")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_error_response_content() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new ImageServiceIdentify.ResponseContract(null, new(400, "missing required parameter", null)))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("national-map", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);
        await Should.ThrowAsync<ArgumentException>(async () => await handler.Handle(_computation, CancellationToken.None));

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "national-map")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task Should_convert_response_meters_to_feet() {
        const string oneMeter = "1";
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new ImageServiceIdentify.ResponseContract(oneMeter, null))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("national-map", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(_computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "national-map")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );

        var lists = result.ToList();
        result.Count.ShouldBe(1);
        lists[0].Attributes.Keys.Count.ShouldBe(1);
        lists[0].Attributes.Keys.ShouldContain("feet");
        lists[0].Attributes.TryGetValue("feet", out var feet).ShouldBeTrue();
        feet.ShouldBe("3.28084");
    }
    [Fact]
    public async Task Should_return_feet_value_and_meters() {
        const string oneMeter = "1";
        var computation = new RasterElevation.Computation("feet, meters, value ,extra", new(new()) {
            Point = new(1, 2, new(3, null))
        });

        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new ImageServiceIdentify.ResponseContract(oneMeter, null))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("national-map", handlerMock.Object, _baseUrl);

        var handler = new RasterElevation.Handler(httpClientFactory.Object, _logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "national-map")), Times.Once);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );

        var lists = result.ToList();
        result.Count.ShouldBe(1);
        lists[0].Attributes.Keys.Count.ShouldBe(3);
        lists[0].Attributes.Keys.ShouldContain("feet");
        lists[0].Attributes.Keys.ShouldContain("meters");
        lists[0].Attributes.Keys.ShouldContain("value");
        lists[0].Attributes.TryGetValue("feet", out var feet).ShouldBeTrue();
        lists[0].Attributes.TryGetValue("meters", out var meters).ShouldBeTrue();
        lists[0].Attributes.TryGetValue("value", out var value).ShouldBeTrue();
        feet.ShouldBe("3.28084");
        meters.ShouldBe("1");
        value.ShouldBe("1");
    }
}
