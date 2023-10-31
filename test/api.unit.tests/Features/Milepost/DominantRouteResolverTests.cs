using System.Net;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Features.Milepost;
public static class DominantRouteResolverTests {
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

            var response = new Concurrencies.ResponseContract([
                    new Concurrencies.ResponseLocations([
                            new Concurrencies.ConcurrencyLocations(true,"1", -1, 1)
                        ], "1", -1, 1),
                    new Concurrencies.ResponseLocations(Array.Empty<Concurrencies.ConcurrencyLocations>(), "2", -1, 1),
                    new Concurrencies.ResponseLocations(Array.Empty<Concurrencies.ConcurrencyLocations>(), "3", -1, 1)
            ], null);

            var computation = new DominantRouteResolver.Computation(locations, null, 0);

            var httpHandler = new Mock<TestingHttpMessageHandler> { CallBase = true };

            httpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response))
            }).Verifiable();

            var client = new HttpClient(httpHandler.Object) {
                BaseAddress = new Uri("https://testing.me")
            };

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient("udot")).Returns(client);

            var log = new Mock<ILogger> { DefaultValue = DefaultValue.Mock };
            var handler = new DominantRouteResolver.Handler(factory.Object, new PythagoreanDistance(), log.Object);

            var result = await handler.Handle(computation, default);

            httpHandler.Verify(x => x.Send(It.IsAny<HttpRequestMessage>()), Times.Exactly(1));
        }

        [Fact]
        public void Should_create_request_with_measures_for_every_location() { }
    }

    public class DominantRouteDescriptorComparerTests {
        [Fact]
        public void Should_order_dominant_then_distance() { }
    }

    public class TestingHttpMessageHandler : HttpMessageHandler {
        public virtual HttpResponseMessage Send(HttpRequestMessage request) =>
            throw new NotImplementedException("Now we can setup this method with our mocking framework");

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) =>
                Task.FromResult(Send(request));
    }
}
