using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Converting;
using api.mapserv.utah.gov.Features.Health;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using EsriJson.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Shouldly;
using Xunit;
using Point = EsriJson.Net.Geometry.Point;

namespace api.tests.Filters {

    internal class TestController : Controller {

    }

    public class JsonOutputFormatTests {
        private static Contexts CreateContext(DefaultHttpContext httpContext) {
            var routeData = new RouteData();
            var actionDescription = new ActionDescriptor();

            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var filterMetadata = new IFilterMetadata[0];

            var actionResult = new ObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse>());
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

            var mediator = new Mock<IMediator>();

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContainer<GeocodeAddressApiResponse>>();
            result2.Value.ShouldBeOfType<ApiResponseContainer<GeocodeAddressApiResponse>>();
        }

        [Fact]
        public async Task Should_call_esrijson() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?format=esriJSON");

            var mediator = new Mock<IMediator>();
            var response = new ApiResponseContainer<Graphic> {
                Result = new Graphic(new Point(2, 2), new Dictionary<string, object>())
            };

            mediator.Setup(x => x.Send(It.IsAny<EsriGraphic.Command>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(response));

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContainer<Graphic>>();
            result2.Value.ShouldBeOfType<ApiResponseContainer<Graphic>>();
        }

        [Fact]
        public async Task Should_call_geojson() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?format=GeoJSON");

            var mediator = new Mock<IMediator>();
            var response = new ApiResponseContainer<Feature> {
                Result = new Feature(new GeoJSON.Net.Geometry.Point(new Position(1, 1)))
            };

            mediator.Setup(x => x.Send(It.IsAny<GeoJsonFeature.Command>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(response));

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContainer<Feature>>();
            result2.Value.ShouldBeOfType<ApiResponseContainer<Feature>>();
        }

        [Fact]
        public async Task Should_skip_if_format_is_empty() {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?test=1");

            var mediator = new Mock<IMediator>();

            var filter = new JsonOutputFormatResultFilter(mediator.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResultExecutionAsync(contexts.ExecutingContext,
                                                () => Task.FromResult(contexts.ExecutedContext));

            var result = contexts.ExecutingContext.Result as ObjectResult;
            var result2 = contexts.ExecutedContext.Result as ObjectResult;

            result.Value.ShouldBeOfType<ApiResponseContainer<GeocodeAddressApiResponse>>();
            result2.Value.ShouldBeOfType<ApiResponseContainer<GeocodeAddressApiResponse>>();
        }
    }
}
