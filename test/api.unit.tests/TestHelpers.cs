using System.Net;
using Moq.Protected;

namespace api.tests.Helpers;
public static class TestHelpers {
    public static HttpContext CreateVersionedHttpContext(int version) {
        var apiVersion = new ApiVersion(version, 0);
        var httpContext = new DefaultHttpContext();

        httpContext.Features.Set<IApiVersioningFeature>(new ApiVersioningFeature(httpContext) {
            RequestedApiVersion = apiVersion
        });

        return httpContext;
    }
    public static HttpContext GenerateContextFor(string query, int version) {
        var context = CreateVersionedHttpContext(version);
        context.Request.QueryString = new QueryString(query);

        return context;
    }

    public static EndpointFilterInvocationContext GetEndpointContext(params object[] arguments) =>
      new DefaultEndpointFilterInvocationContext(new DefaultHttpContext(), arguments);

    public static Mock<IHttpClientFactory> CreateHttpClientFactory(string clientName, HttpMessageHandler handler, string baseAddress = "http://localhost") {
        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri(baseAddress),
        };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.Is<string>((x) => x == clientName))).Returns(httpClient);

        return httpClientFactory;
    }

    internal static Mock<HttpMessageHandler> CreateHttpMessageHandler(HttpContent content) {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
               Content = content
           })
           .Verifiable();

        return handlerMock;
    }
    internal static Mock<HttpMessageHandler> CreateHttpMessageHandlerThatThrows<T>(T exception) where T : Exception {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ThrowsAsync(exception)
           .Verifiable();

        return handlerMock;
    }
}
