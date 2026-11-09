using BenchmarkDotNet.Attributes;
using StackExchange.Redis;
using ugrc.api.Features.Converting;
using ugrc.api.Middleware;
using ugrc.api.Models;

namespace benchmark.api;

/// <summary>
/// The api key filter runs on every request so any cost added here is paid by every caller
/// </summary>
public class AuthorizeApiKeyBenchmarks {
    private static readonly ValueTask<object?> _next = ValueTask.FromResult<object?>(null);

    private AuthorizeApiKeyFilter _browserFilter = default!;
    private AuthorizeApiKeyFilter _serverFilter = default!;
    private DefaultHttpContext _browserContext = default!;
    private DefaultHttpContext _serverContext = default!;

    [GlobalSetup]
    public void Setup() {
        var redis = new Lazy<IConnectionMultiplexer>(NullProxy.Create<IConnectionMultiplexer>);
        var factory = new JsonSerializerOptionsFactory();

        // the shape of the pattern the self service website generates for `*.utah.gov/*`
        var browserKey = new ApiKey("AGRC-BROWSER") {
            RegularExpression = @"^https?:\/\/[a-z0-9._-]+\.utah\.gov",
        };
        browserKey.Flags["production"] = true;

        var serverKey = new ApiKey("AGRC-SERVER") {
            Pattern = "10.0.0.1",
        };
        serverKey.Flags["server"] = true;

        _browserFilter = new AuthorizeApiKeyFilter(null!, new QueryStringKeyProvider(), new ForwardedForIpProvider(),
            new FakeApiKeyRepository(browserKey), redis, factory);
        _serverFilter = new AuthorizeApiKeyFilter(null!, new QueryStringKeyProvider(), new ForwardedForIpProvider(),
            new FakeApiKeyRepository(serverKey), redis, factory);

        _browserContext = new DefaultHttpContext();
        _browserContext.Request.QueryString = new QueryString("?apikey=AGRC-BROWSER");
        _browserContext.Request.Headers.Referer = "https://gis.utah.gov/products/sgid/address/";
        _browserContext.Request.Headers.Origin = "https://gis.utah.gov";

        _serverContext = new DefaultHttpContext();
        _serverContext.Request.QueryString = new QueryString("?apikey=AGRC-SERVER");
        _serverContext.Request.Headers["X-Forwarded-For"] = "10.0.0.1";
    }

    [Benchmark]
    public ValueTask<object?> BrowserKey() =>
        _browserFilter.InvokeAsync(new DefaultEndpointFilterInvocationContext(_browserContext), _ => _next);

    [Benchmark]
    public ValueTask<object?> ServerKey() =>
        _serverFilter.InvokeAsync(new DefaultEndpointFilterInvocationContext(_serverContext), _ => _next);
}
