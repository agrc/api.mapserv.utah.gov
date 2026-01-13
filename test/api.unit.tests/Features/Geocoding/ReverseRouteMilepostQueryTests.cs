using System.Net.Http.Json;
using Moq.Protected;
using ugrc.api.Features.Milepost;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;
using ugrc.api.Models.ResponseContracts;

namespace api.tests.Features.Geocoding;
public class ReverseRouteMilepostQueryTests {
    private readonly ILogger _logger;
    private readonly IComputeMediator _computeMediator;
    private readonly Uri _expectedUri = new("""https://roads.udot.utah.gov/server/rest/services/LrsEnabled/Read_Only_Public_LRS_Routes/MapServer/exts/LRServer/networkLayers/1/geometryToMeasure?f=json&locations=%5B{"geometry"%3A{"x"%3A423692,"y"%3A4499779}}%5D&outSR=26912&inSR=26912&tolerance=100""");
    private readonly ReverseRouteMilepostQuery.Query _query = new(423692, 4499779, new());
    public ReverseRouteMilepostQueryTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        var computeMediator = new Mock<IComputeMediator>();
        computeMediator.Setup(x => x.Handle(It.IsAny<DominantRouteResolver.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DominantRouteResolver.Computation computation, CancellationToken _) =>
                new ReverseRouteMilepostResponseContract {
                    OffsetMeters = .01,
                    Route = computation._routeMap.First().Key,
                    Milepost = computation._routeMap.First().Value.Measure,
                    Dominant = true
                }
            );
        _computeMediator = computeMediator.Object;
    }

    [Fact]
    public async Task Should_handle_task_canceled_exceptions() {
        var handlerMock = TestHelpers.CreateHttpMessageHandlerThatThrows(new TaskCanceledException());
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
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
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
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
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
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
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new GeometryToMeasure.ResponseContract(null, new(400, "missing required parameter", null))));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
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
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new GeometryToMeasure.ResponseContract(null, null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
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
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(new GeometryToMeasure.ResponseContract([
            new GeometryToMeasure.ResponseLocation {
                Status = GeometryToMeasure.Status.esriLocatingCannotFindRoute,
                Results = [
                    new GeometryToMeasure.ResponseLocation{
                        RouteId = "0015PM",
                        Measure = 123.456789,
                        Geometry = new Point(1, 2)
                    }
                ]
            },
        ], null)));
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
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
    public async Task Should_returns_first_dominant_location() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new GeometryToMeasure.ResponseContract([
                new GeometryToMeasure.ResponseLocation {
                    Status = GeometryToMeasure.Status.esriLocatingOK,
                    Results = [
                        new GeometryToMeasure.ResponseLocation{
                            RouteId = "12TVK22729954_GREENOAKS_DR",
                            Measure = 987.654321,
                            Geometry = new Point(3, 4)
                        },
                        new GeometryToMeasure.ResponseLocation{
                            RouteId = "0015PM",
                            Measure = 300,
                            Geometry = new Point(1, 2)
                        },
                    ]
                },
            ], null))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status200OK);
        result.Result.ShouldBeAssignableTo<ReverseRouteMilepostResponseContract>();

        var response = result.Result as ReverseRouteMilepostResponseContract;
        response.Milepost.ShouldBe(300);
        response.Route.ShouldBe("15P");
        response.OffsetMeters.ShouldBe(0);
        response.Side.ShouldBe("increasing");
        response.Dominant.ShouldBe(true);

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
    [Fact]
    public async Task Should_handle_multiple_locations() {
        var handlerMock = TestHelpers.CreateHttpMessageHandler(JsonContent.Create(
            new GeometryToMeasure.ResponseContract([
                new GeometryToMeasure.ResponseLocation {
                    Status = GeometryToMeasure.Status.esriLocatingMultipleLocation,
                    Results = [
                        new GeometryToMeasure.ResponseLocation{
                            RouteId = "0015PM",
                            Measure = 300,

                            Geometry = new Point(1, 2)
                        },
                        new GeometryToMeasure.ResponseLocation{
                            RouteId = "0215PM",
                            Measure = 987.654321,
                            Geometry = new Point(3, 4)
                        }
                    ]
                },
            ], null))
        );
        var httpClientFactory = TestHelpers.CreateHttpClientFactory("udot", handlerMock.Object, "https://roads.udot.utah.gov/");

        var handler = new ReverseRouteMilepostQuery.Handler(_computeMediator, httpClientFactory.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        httpClientFactory.Verify(x => x.CreateClient(It.Is<string>((x) => x == "udot")), Times.Once);
        result.Status.ShouldBe(StatusCodes.Status200OK);
        result.Result.ShouldBeAssignableTo<ReverseRouteMilepostResponseContract>();

        var response = result.Result as ReverseRouteMilepostResponseContract;
        response.Milepost.ShouldBe(300);
        response.Route.ShouldBe("15P");
        response.OffsetMeters.ShouldBe(.01);
        response.Side.ShouldBe("increasing");
        response.Dominant.ShouldBe(true);

        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
           ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == _expectedUri),
           ItExpr.IsAny<CancellationToken>()
        );
    }
}
