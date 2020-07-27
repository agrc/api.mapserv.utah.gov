using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.GeometryService;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.GeometryService {

    public class FakeHttpMessageHandler : HttpMessageHandler {
        public HttpRequestMessage RequestMessage { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            RequestMessage = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{}")
            });
        }
    }

    public class ReprojectTests {
        private readonly ILogger _log;

        public ReprojectTests() {
            var logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

            _log = logger.Object;
        }

        [Fact]
        public async Task Should_prefer_request_url() {
            var command = new Reproject.Computation(new PointReprojectOptions(0, 0, new[] { 0.0, 1.1 })) {
                ReprojectUrl = "http://useme"
            };

            var options = new Mock<IOptions<GeometryServiceConfiguration>>();
            options.Setup(x => x.Value).Returns(new GeometryServiceConfiguration {
                Host = "options"
            });

            var messageHandler = new FakeHttpMessageHandler();
            var client = new HttpClient(messageHandler);

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var handler = new Reproject.Handler(options.Object, factory.Object, _log);

            var response = await handler.Handle(command, CancellationToken.None);

            messageHandler.RequestMessage.RequestUri.Host.ShouldBe("useme");
        }

        [Fact]
        public async Task Should_fall_back_to_options() {
            var command = new Reproject.Computation(new PointReprojectOptions(0, 0, new[] { 0.0, 1.1 }));

            var options = new Mock<IOptions<GeometryServiceConfiguration>>();
            options.Setup(x => x.Value).Returns(new GeometryServiceConfiguration {
                Host = "options"
            });

            var messageHandler = new FakeHttpMessageHandler();
            var client = new HttpClient(messageHandler);

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var handler = new Reproject.Handler(options.Object, factory.Object, _log);

            var response = await handler.Handle(command, CancellationToken.None);

            messageHandler.RequestMessage.RequestUri.Host.ShouldBe("options");
        }

        [Fact]
        public async Task Should_format_query_string() {
            var command = new Reproject.Computation(new PointReprojectOptions(0, 1, new[] { 1.1, 2.2 }));

            var options = new Mock<IOptions<GeometryServiceConfiguration>>();
            options.Setup(x => x.Value).Returns(new GeometryServiceConfiguration {
                Host = "options"
            });

            var messageHandler = new FakeHttpMessageHandler();
            var client = new HttpClient(messageHandler);

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var handler = new Reproject.Handler(options.Object, factory.Object, _log);

            var response = await handler.Handle(command, CancellationToken.None);

            messageHandler.RequestMessage.RequestUri.Query.ShouldBe("?f=json&inSR=0&outSR=1&geometries=1.1,2.2");
        }
    }
}
