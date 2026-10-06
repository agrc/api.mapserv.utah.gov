using Serilog.Core;
using Serilog.Events;
using ugrc.api.Middleware;
using ugrc.api.Services;

namespace api.tests.Filters;
public class RequestLoggerEndpointTests {
    private class CollectingSink : ILogEventSink {
        public List<LogEvent> Events { get; } = [];

        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
    }

    [Theory]
    [InlineData("/a", "-")]
    [InlineData("/api", "-")]
    [InlineData("/api/v1", "-")]
    [InlineData("/robots", "-")]
    [InlineData("/", "-")]
    [InlineData("/apis/v1/search", "-")]
    [InlineData("/api/v1/", "unknown")]
    [InlineData("/abcdef/", "error")]
    [InlineData("/api/v1/search/table/field", "search")]
    [InlineData("/api/v1/health", "health")]
    [InlineData("/api/v1/geocode/123 main/84111", "geocode")]
    [InlineData("/api/v1/geocode/reverse/1/2", "reverse geocode")]
    [InlineData("/api/v1/geocode/reversemilepost/1/2", "reverse milepost")]
    [InlineData("/api/v1/geocode/milepost/15/100", "milepost")]
    [InlineData("/api/v1/info/featureclassnames", "table info")]
    [InlineData("/api/v1/info/fieldnames/table", "field info")]
    [InlineData("/api/v1/info", "info")]
    [InlineData("/API/V1/SEARCH/table/field", "search")]
    public async Task Should_log_the_endpoint_without_throwing(string path, string expected) {
        var sink = new CollectingSink();
        var log = new LoggerConfiguration().WriteTo.Sink(sink).CreateLogger();

        var keyProvider = new Mock<IBrowserKeyProvider>();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = path;

        var middleware = new RequestLoggerMiddleware(context => {
            context.Response.StatusCode = 404;

            return Task.CompletedTask;
        }, log, keyProvider.Object);

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.StatusCode.ShouldBe(404);

        var logEvent = sink.Events.ShouldHaveSingleItem();
        logEvent.Properties["endpoint"].ShouldBeOfType<ScalarValue>().Value.ShouldBe(expected);
    }
}
