// using ugrc.api.Features.Geocoding;
// using ugrc.api.Infrastructure;
// using ugrc.api.Models.ResponseContracts;
// using ugrc.api.Quirks;
// using EsriJson.Net;
// using NetTopologySuite.Features;
// using Point = EsriJson.Net.Geometry.Point;

// namespace api.tests.Filters;
// public class JsonOutputFormatTests {
//     [Fact]
//     public async Task Should_be_ok_if_format_is_unknown() {
//         var httpContext = HttpContextHelpers.CreateVersionedHttpContext(1);
//         httpContext.Request.QueryString = new QueryString("?format=someUnknownFormat");

//         var filter = new JsonpMiddleware(new Mock<RequestDelegate>().Object);

//         await filter.InvokeAsync(httpContext);
//     }

//     [Fact]
//     public async Task Should_call_esrijson() {
//         var httpContext = HttpContextHelpers.CreateVersionedHttpContext(1);

//         httpContext.Request.QueryString = new QueryString("?format=esriJSON");
//         var contexts = HttpContextHelpers.CreateContext(httpContext);

//         var httpContextAccessor = new Mock<IHttpContextAccessor>();
//         httpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

//         var mediator = new Mock<IComputeMediator>();
//         var response = new ApiResponseContract<SerializableGraphic> {
//             Result = new SerializableGraphic(new Graphic(new Point(2, 2), new Dictionary<string, object>()))
//         };

//         mediator.Setup(x => x.Handle(It.IsAny<Computation>(), It.IsAny<CancellationToken>()))
//                 .Returns(Task.FromResult(response));

//         var filter = new JsonOutputFormatResultFilter(mediator.Object);

//         await filter.OnResultExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

//         var result = contexts.ExecutingContext.Result as ObjectResult;
//         var result2 = contexts.ExecutedContext.Result as ObjectResult;

//         ArgumentNullException.ThrowIfNull(result);
//         ArgumentNullException.ThrowIfNull(result2);

//         result.Value.ShouldBeOfType<ApiResponseContract<SerializableGraphic>>();
//         result2.Value.ShouldBeOfType<ApiResponseContract<SerializableGraphic>>();
//     }

//     [Fact]
//     public async Task Should_call_geojson() {
//         var httpContext = HttpContextHelpers.CreateVersionedHttpContext(1);

//         httpContext.Request.QueryString = new QueryString("?format=GeoJSON");
//         var contexts = HttpContextHelpers.CreateContext(httpContext);

//         var httpContextAccessor = new Mock<IHttpContextAccessor>();
//         httpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

//         var mediator = new Mock<IComputeMediator>();
//         var response = new ApiResponseContract<Feature> {
//             Result = new Feature(new NetTopologySuite.Geometries.Point(1, 1), new AttributesTable())
//         };

//         mediator.Setup(x => x.Handle(It.IsAny<GeoJsonFeature.Computation>(), It.IsAny<CancellationToken>()))
//                 .Returns(Task.FromResult(response));

//         var filter = new JsonOutputFormatResultFilter(mediator.Object);

//         await filter.OnResultExecutionAsync(contexts.ExecutingContext,
//                                             () => Task.FromResult(contexts.ExecutedContext));

//         var result = contexts.ExecutingContext.Result as ObjectResult;
//         var result2 = contexts.ExecutedContext.Result as ObjectResult;

//         ArgumentNullException.ThrowIfNull(result);
//         ArgumentNullException.ThrowIfNull(result2);

//         result.Value.ShouldBeOfType<ApiResponseContract<Feature>>();
//         result2.Value.ShouldBeOfType<ApiResponseContract<Feature>>();
//     }

//     [Fact]
//     public async Task Should_skip_if_format_is_empty() {
//         var httpContext = HttpContextHelpers.CreateVersionedHttpContext(1);
//         httpContext.Request.QueryString = new QueryString("?test=1");

//         var httpContextAccessor = new Mock<IHttpContextAccessor>();
//         httpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

//         var mediator = new Mock<IComputeMediator>();

//         var filter = new JsonOutputFormatResultFilter(mediator.Object);

//         var contexts = HttpContextHelpers.CreateContext(httpContext);
//         await filter.OnResultExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

//         var result = contexts.ExecutingContext.Result as ObjectResult;
//         var result2 = contexts.ExecutedContext.Result as ObjectResult;

//         ArgumentNullException.ThrowIfNull(result);
//         ArgumentNullException.ThrowIfNull(result2);

//         result.Value.ShouldBeOfType<ApiResponseContract<SingleGeocodeResponseContract>>();
//         result2.Value.ShouldBeOfType<ApiResponseContract<SingleGeocodeResponseContract>>();
//     }
// }
