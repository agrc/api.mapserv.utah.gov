using Serilog.Core;
using Serilog.Events;
using ugrc.api.Middleware;
using ugrc.api.Services;

namespace api.tests.Filters;
public class RequestLoggerTests {
    private class CollectingSink : ILogEventSink {
        public List<LogEvent> Events { get; } = [];

        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
    }

    private static async Task<IReadOnlyDictionary<string, LogEventPropertyValue>> Invoke(Action<HttpContext> configure, int statusCode = 200) {
        var sink = new CollectingSink();
        var log = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>())).Returns("AGRC-Key");

        var httpContext = new DefaultHttpContext();
        configure(httpContext);

        var middleware = new RequestLoggerMiddleware(context => {
            context.Response.StatusCode = statusCode;

            return Task.CompletedTask;
        }, log, keyProvider.Object);

        await middleware.InvokeAsync(httpContext);

        var logEvent = sink.Events.ShouldHaveSingleItem();
        logEvent.MessageTemplate.Text.ShouldBe("Analytics:request");

        return logEvent.Properties;
    }

    private static object Scalar(IReadOnlyDictionary<string, LogEventPropertyValue> properties, string name) =>
        properties[name].ShouldBeOfType<ScalarValue>().Value;

    [Fact]
    public async Task Should_log_request_analytics() {
        var properties = await Invoke(context => {
            context.Request.Path = "/api/v1/geocode/123 main/84111";
            context.Request.Headers.Referer = "https://example.com/map?token=secret#frag";
            context.Request.Headers.Origin = "https://example.com";
            context.Request.Headers.UserAgent = "test-agent";
        }, 404);

        Scalar(properties, "key").ShouldBe("AGRC-Key");
        Scalar(properties, "endpoint").ShouldBe("geocode");
        Scalar(properties, "version").ShouldBe("v1");
        Scalar(properties, "result").ShouldBe(404);
        Scalar(properties, "duration").ShouldBeOfType<double>().ShouldBeGreaterThanOrEqualTo(0);
        Scalar(properties, "referer").ShouldBe("https://example.com/map");
        Scalar(properties, "origin").ShouldBe("https://example.com");
        Scalar(properties, "userAgent").ShouldBe("test-agent");
    }

    [Fact]
    public async Task Should_log_null_for_missing_headers() {
        var properties = await Invoke(context => context.Request.Path = "/api/v1/search/table/field");

        Scalar(properties, "referer").ShouldBeNull();
        Scalar(properties, "origin").ShouldBeNull();
        Scalar(properties, "userAgent").ShouldBeNull();
    }

    [Theory]
    [InlineData("/api/v1/search/table/field", "v1")]
    [InlineData("/API/V2/search/table/field", "v2")]
    [InlineData("/api/12/search/table/field", null)]
    [InlineData("/other/v1/search/table/field", null)]
    [InlineData("/abcdefghij", null)]
    [InlineData("", null)]
    public async Task Should_parse_version(string path, string expected) {
        var properties = await Invoke(context => context.Request.Path = path);

        Scalar(properties, "version").ShouldBe(expected);
    }

    [Theory]
    [InlineData("https://example.com/path/page.html?token=secret#frag", "https://example.com/path/page.html")]
    [InlineData("https://example.com", "https://example.com/")]
    [InlineData("/relative/page?token=secret", "/relative/page")]
    [InlineData("not a uri#frag", "not a uri")]
    [InlineData("no-query-or-fragment", "no-query-or-fragment")]
    public async Task Should_strip_query_and_fragment_from_referer(string referer, string expected) {
        var properties = await Invoke(context => {
            context.Request.Path = "/api/v1/search/table/field";
            context.Request.Headers.Referer = referer;
        });

        Scalar(properties, "referer").ShouldBe(expected);
    }

    [Theory]
    [InlineData("/api/v1/search/table/field", "search")]
    [InlineData("/api/v1/health/details", "health")]
    [InlineData("/api/v1/geocode/reverse/1/2", "reverse geocode")]
    [InlineData("/api/v1/geocode/reversemilepost/1/2", "reverse milepost")]
    [InlineData("/api/v1/geocode/milepost/15/100", "milepost")]
    [InlineData("/api/v1/info/featureclassnames", "table info")]
    [InlineData("/api/v1/info/fieldnames/table", "field info")]
    [InlineData("/api/v1/info", "info")]
    [InlineData("/api/v1/other", "unknown")]
    [InlineData("/abcdef/", "error")]
    [InlineData("/apis/v1/search", "-")]
    [InlineData("/", "-")]
    public async Task Should_parse_endpoint(string path, string expected) {
        var properties = await Invoke(context => context.Request.Path = path);

        Scalar(properties, "endpoint").ShouldBe(expected);
    }
}
