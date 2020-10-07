using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Filters;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using EsriJson.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NetTopologySuite.Features;
using Shouldly;
using Xunit;
using static AGRC.api.Features.Converting.EsriGraphic;
using Point = EsriJson.Net.Geometry.Point;

namespace api.tests.Filters {

    internal class TestController : Controller {

    }

    public class JsonOutputFormatTests {
        private static Contexts CreateContext(DefaultHttpContext httpContext) {
            var routeData = new RouteData();
            var actionDescription = new ActionDescriptor();

            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var filterMetadata = System.Array.Empty<IFilterMetadata>();

            var actionResult = new ObjectResult(new ApiResponseContract<SingleGeocodeResponseContract>());
            var controller = new TestController();

            var context = new ResultExecutingContext(actionContext, filterMetadata, actionResult, controller);
            var resultContext = new ResultExecutedContext(actionContext, filterMetadata, actionResult, controller);

            return new Contexts(context, resultContext);
        }

        private class Contexts {
            public Contexts(ResultExecutingContext c1, ResultExecutedContext c2) {
                ExecutingContext = c1;
                ExecutedContext = c2;
            }

            public ResultExecutingContext ExecutingContext { get; }
            public ResultExecutedContext ExecutedContext { get; }
        }


        [Fact]
        public async Task Should_be_ok_if_format_is_unknown() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?format=someUnknownFormat");

            var mediator = new Mock<IComputeMediator>();

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContract<SingleGeocodeResponseContract>>();
            result2.Value.ShouldBeOfType<ApiResponseContract<SingleGeocodeResponseContract>>();
        }

        [Fact]
        public async Task Should_call_esrijson() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?format=esriJSON");

            var mediator = new Mock<IComputeMediator>();
            var response = new ApiResponseContract<SerializableGraphic> {
                Result = new SerializableGraphic(new Graphic(new Point(2, 2), new Dictionary<string, object>()))
            };

            mediator.Setup(x => x.Handle(It.IsAny<Computation>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(response));

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContract<SerializableGraphic>>();
            result2.Value.ShouldBeOfType<ApiResponseContract<SerializableGraphic>>();
        }

        [Fact]
        public async Task Should_call_geojson() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?format=GeoJSON");

            var mediator = new Mock<IComputeMediator>();
            var response = new ApiResponseContract<Feature> {
                Result = new Feature(new NetTopologySuite.Geometries.Point(1, 1), new AttributesTable())
            };

            mediator.Setup(x => x.Handle(It.IsAny<GeoJsonFeature.Computation>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(response));

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContract<Feature>>();
            result2.Value.ShouldBeOfType<ApiResponseContract<Feature>>();
        }

        [Fact]
        public async Task Should_skip_if_format_is_empty() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?test=1");

            var mediator = new Mock<IComputeMediator>();

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContract<SingleGeocodeResponseContract>>();
            result2.Value.ShouldBeOfType<ApiResponseContract<SingleGeocodeResponseContract>>();
        }
    }
}
