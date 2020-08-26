using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models.ArcGis;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Serilog;
using Xunit;

namespace AGRC.api.Features.Milepost {
    public class DominantRouteResolverTests {
        public class ComputationTests {
            [Fact]
            public async Task Should_build_route_map_for_every_location() {
                var locations = new[] {
                    new GeometryToMeasure.ResponseLocation {
                        RouteId = "1",
                        Measure = 0
                    },
                    new GeometryToMeasure.ResponseLocation {
                        RouteId = "2",
                        Measure = 0
                    },
                    new GeometryToMeasure.ResponseLocation {
                        RouteId = "3",
                        Measure = 0
                    }
                };

                var computation = new DominantRouteResolver.Computation(locations, null, 0);

                var httpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                httpHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("GetAsync",
                        ItExpr.IsAny<string>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(new Concurrencies.ResponseContract()))
                    }).Verifiable();

                var client = new HttpClient(httpHandler.Object);

                var factory = new Mock<IHttpClientFactory>();
                factory.Setup(x => x.CreateClient("udot")).Returns(client);

                var log = new Mock<ILogger> { DefaultValue = DefaultValue.Mock };

                var handler = new DominantRouteResolver.Handler(factory.Object, new PythagoreanDistance(), log.Object);

                var result = await handler.Handle(computation, default);

                httpHandler.Protected().Verify("GetAsync", Times.Exactly(3));
            }

            [Fact]
            public void Should_create_request_with_measures_for_every_location() {

            }
        }

        public class DominantRouteDescriptorComparerTests {
            [Fact]
            public void Should_order_dominant_then_distance() {

            }
        }
    }
}
