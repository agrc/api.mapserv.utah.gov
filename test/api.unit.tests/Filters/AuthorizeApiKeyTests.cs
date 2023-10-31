using AGRC.api.Middleware;
using AGRC.api.Models;
using AGRC.api.Models.ResponseContracts;
using AGRC.api.Services;

namespace api.tests.Filters;
public class AuthorizeApiKeyTests {
    private readonly ILogger _log;

    public AuthorizeApiKeyTests() {
        var logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        _log = logger.Object;
    }

    [Theory]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test/index.html?query=yes",
        null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test/index.html?query=yes",
        400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/reqes/test.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/request/test/index.html?query=yes",
        null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test/index.html?query=yes",
        400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/", "http://www.example.com", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/", "http://www.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://example\.com/.*", "http://example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://example\.com/.*", "http://example.com/request/index.html", null)]
    [InlineData(@"^htt(p|ps)://example\.com\/.*", "http://bad.example.com/index.html", 400)]
    [InlineData(@"^htt(p|ps)://example\.com\/.*", "http://bad.example.com/request/index.html", 400)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/", null)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/request/test.html", null)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/request/test/index.html?query=yes", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/test/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/test", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/bad", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/bad/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://bad.example.com/test/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/test/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/test/test2/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://bad.example.com/test/test/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/bad/test2/index.htm", 400)]
    [InlineData(@"^htt(p|ps)://.+\.nedds\.health\.utah\.gov*", "http://www.nedds.health.utah.gov", null)]
    [InlineData(@"^htt(p|ps)://api\.utlegislators\.com", "http://api.utlegislators.com", null)]
    [InlineData(@"^htt(p|ps)://168\.177\.222\.22\/app\/.*", "http://168.177.222.22/app/whatever", null)]
    public async Task Should_validate_production_browser_key(string pattern, string url, object responseCode) {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("Api-Key");

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.Is<string>(p => p.Equals("Api-Key"))))
               .ReturnsAsync(new ApiKey("Api-Key") {
                   Elevated = false,
                   Flags = new() { { "deleted", false }, { "disabled", false }, { "production", true }, { "server", false } },
                   RegularExpression = pattern,
               });

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Referrer"] = url;

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(responseCode);
        } else {
            result.ShouldBe(responseCode);
        }
    }

    [Theory]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test/index.html?query=yes",
        null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test/index.html?query=yes",
        400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/reqes/test.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/request/test/index.html?query=yes",
        null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test/index.html?query=yes",
        400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/", "http://www.example.com", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/", "http://www.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://example\.com/.*", "http://example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://example\.com/.*", "http://example.com/request/index.html", null)]
    [InlineData(@"^htt(p|ps)://example\.com\/.*", "http://bad.example.com/index.html", 400)]
    [InlineData(@"^htt(p|ps)://example\.com\/.*", "http://bad.example.com/request/index.html", 400)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/", null)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/index.html", null)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/request/test.html", null)]
    [InlineData(@"^htt(p|ps)://.+\.example\.com", "http://any.example.com/request/test/index.html?query=yes", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/test/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/test", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/bad", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://www.example.com/bad/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test", "http://bad.example.com/test/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/test/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/test/test2/index.html", null)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://bad.example.com/test/test/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/bad/test2/index.htm", 400)]
    [InlineData(@"^htt(p|ps)://.+\.nedds\.health\.utah\.gov*", "http://www.nedds.health.utah.gov", null)]
    [InlineData(@"^htt(p|ps)://api\.utlegislators\.com", "http://api.utlegislators.com", null)]
    [InlineData(@"^htt(p|ps)://168\.177\.222\.22\/app\/.*", "http://168.177.222.22/app/whatever", null)]
    public async Task Should_validate_production_browser_key_with_cors_header(
        string pattern, string url, object responseCode) {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("Api-Key");

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.Is<string>(p => p.Equals("Api-Key"))))
               .ReturnsAsync(new ApiKey("Api-Key") {
                   Elevated = false,
                   Flags = new() { { "deleted", false }, { "disabled", false }, { "production", true }, { "server", false } },
                   RegularExpression = pattern,
               });

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Referrer"] = url;
        httpContext.Request.Headers["Origin"] = url;

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(responseCode);
        } else {
            result.ShouldBe(responseCode);
        }
    }

    [Theory]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://bad.example.com/test/test/index.html", 400)]
    [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/bad/test2/index.htm", 400)]
    [InlineData(@"^htt(p|ps)://machine-name\/.*", "http://machine-name/beta/index.htm", null)]
    [InlineData("^htt(p|ps)://machine-name", "http://machine-name/index.html", null)]
    public async Task Should_validate_dev_key(string pattern, string url, object responseCode) {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("Api-Key");

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.Is<string>(p => p.Equals("Api-Key"))))
               .ReturnsAsync(new ApiKey("Api-Key") {
                   Elevated = false,
                   Flags = new() { { "deleted", false }, { "disabled", false }, { "production", false }, { "server", false } },
                   RegularExpression = pattern,
               });

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Referrer"] = url;

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(responseCode);
        } else {
            result.ShouldBe(responseCode);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Should_404_empty_key(string key) {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns(key);

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
               .ReturnsAsync((ApiKey)null);

        var httpContext = new DefaultHttpContext();

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(400);
        } else {
            result.ShouldBe(false);
        }
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task Should_404_disabled_or_deleted_keys(bool deleted, bool disabled) {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("key");

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
               .ReturnsAsync(new ApiKey("key") {
                   Elevated = false,
                   Flags = new() { { "deleted", deleted }, { "disabled", disabled }, { "production", true }, { "server", false } },
                   RegularExpression = "pattern",
               });

        var httpContext = new DefaultHttpContext();

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(400);
        } else {
            result.ShouldBe(false);
        }
    }

    [Theory]
    [InlineData("0.0.0.1", "0.0.0.1", null)]
    [InlineData("1.1.1.1", "0.0.0.1", 400)]
    public async Task Should_validate_ip_based_keys(string ip, string keyIp, object responseCode) {
        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("key");

        var ipProvider = new Mock<IServerIpProvider>();
        ipProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                  .Returns(ip);

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
               .ReturnsAsync(new ApiKey("key") {
                   Elevated = false,
                   Flags = new() { { "deleted", false }, { "disabled", false }, { "production", true }, { "server", true } },
                   Pattern = keyIp,
               });

        var httpContext = new DefaultHttpContext();

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(responseCode);
        } else {
            result.ShouldBe(responseCode);
        }
    }

    [Fact]
    public async Task Should_404_no_key() {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("key");

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
               .ReturnsAsync((ApiKey)null);

        var httpContext = new DefaultHttpContext();

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        if (result is ApiResponseContract contract) {
            contract.Status.ShouldBe(400);
        } else {
            result.ShouldBe(false);
        }
    }

    [Fact]
    public async Task Should_pass_elevated_keys() {
        var ipProvider = new Mock<IServerIpProvider>();

        var keyProvider = new Mock<IBrowserKeyProvider>();
        keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                   .Returns("key");

        var apiRepo = new Mock<IApiKeyRepository>();
        apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
               .ReturnsAsync(new ApiKey("key") {
                   Elevated = true,
                   Flags = new() { { "deleted", false }, { "disabled", false }, { "production", true }, { "server", false } },
                   RegularExpression = "pattern",
               });

        var httpContext = new DefaultHttpContext();

        var filter = new AuthorizeApiKeyFilter(_log, keyProvider.Object, ipProvider.Object, apiRepo.Object);

        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(x => x.HttpContext).Returns(httpContext);

        var result = await filter.InvokeAsync(contextMock.Object, (_) => new ValueTask<object>());

        result.ShouldBeNull();
    }
}
