using System.Collections.Generic;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using Xunit;

namespace api.tests.Filters {
    public class AuthorizeApiKeyTests {
        [Theory]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/index.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test/index.html?query=yes", null)]

        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/index.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test/index.html?query=yes", 400)]

        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/index.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/reqes/test.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/request/test/index.html?query=yes", null)]

        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/index.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test/index.html?query=yes", 400)]

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
                   .Returns(Task.FromResult(new ApiKey("Api-Key") {
                        Whitelisted = false,
                        Deleted = false,
                        Enabled = ApiKey.KeyStatus.Active,
                        Type = ApiKey.ApplicationType.Browser,
                        RegexPattern = pattern,
                        Configuration = ApiKey.ApplicationStatus.Production
                    })
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referrer"] = url;

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(responseCode);
            } else {
                contexts.ExecutingContext.Result.ShouldBe(responseCode);
            }
        }

        [Theory]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/index.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.example.com/request/test/index.html?query=yes", null)]

        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/index.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com", "http://www.badexample.com/request/test/index.html?query=yes", 400)]

        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/index.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/reqes/test.html", null)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.example.com/request/test/index.html?query=yes", null)]

        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/index.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com.*", "http://www.badexample.com/request/test/index.html?query=yes", 400)]

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
        public async Task Should_validate_production_browser_key_with_cors_header(string pattern, string url, object responseCode) {
            var ipProvider = new Mock<IServerIpProvider>();

            var keyProvider = new Mock<IBrowserKeyProvider>();
            keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                       .Returns("Api-Key");

            var apiRepo = new Mock<IApiKeyRepository>();
            apiRepo.Setup(x => x.GetKey(It.Is<string>(p => p.Equals("Api-Key"))))
                   .Returns(Task.FromResult(new ApiKey("Api-Key") {
                       Whitelisted = false,
                       Deleted = false,
                       Enabled = ApiKey.KeyStatus.Active,
                       Type = ApiKey.ApplicationType.Browser,
                       RegexPattern = pattern,
                       Configuration = ApiKey.ApplicationStatus.Production
                   })
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referrer"] = url;
            httpContext.Request.Headers["Origin"] = url;

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(responseCode);
            } else {
                contexts.ExecutingContext.Result.ShouldBe(responseCode);
            }
        }

        [Theory]
        [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://bad.example.com/test/test/index.html", 400)]
        [InlineData(@"^htt(p|ps)://www\.example\.com\/test\/.*", "http://www.example.com/bad/test2/index.htm", 400)]
        [InlineData(@"^htt(p|ps)://machine-name", "http://machine-name/index.html", null)]
        [InlineData(@"^htt(p|ps)://machine-name\/.*", "http://machine-name/beta/index.htm", null)]
        public async Task Should_validate_dev_key(string pattern, string url, object responseCode) {
            var ipProvider = new Mock<IServerIpProvider>();

            var keyProvider = new Mock<IBrowserKeyProvider>();
            keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                       .Returns("Api-Key");

            var apiRepo = new Mock<IApiKeyRepository>();
            apiRepo.Setup(x => x.GetKey(It.Is<string>(p => p.Equals("Api-Key"))))
                   .Returns(Task.FromResult(new ApiKey("Api-Key") {
                       Whitelisted = false,
                       Deleted = false,
                       Enabled = ApiKey.KeyStatus.Active,
                       Type = ApiKey.ApplicationType.Browser,
                       RegexPattern = pattern,
                       Configuration = ApiKey.ApplicationStatus.Development
                   })
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referrer"] = url;

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(responseCode);
            } else {
                contexts.ExecutingContext.Result.ShouldBe(responseCode);
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
                   .Returns(Task.FromResult((ApiKey)null));

            var httpContext = new DefaultHttpContext();

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(400);
            } else {
                true.ShouldBe(false);
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
                   .Returns(Task.FromResult((ApiKey)null));

            var httpContext = new DefaultHttpContext();

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(400);
            } else {
                true.ShouldBe(false);
            }
        }

        [Theory]
        [InlineData(true, ApiKey.KeyStatus.Disabled)]
        [InlineData(false, ApiKey.KeyStatus.Active)]
        public async Task Should_404_disabled_or_deleted_keys(bool deleted, ApiKey.KeyStatus status) {
            var ipProvider = new Mock<IServerIpProvider>();

            var keyProvider = new Mock<IBrowserKeyProvider>();
            keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                       .Returns("key");

            var apiRepo = new Mock<IApiKeyRepository>();
            apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
                   .Returns(Task.FromResult(new ApiKey("key") {
                       Whitelisted = false,
                       Deleted = deleted,
                       Enabled = status,
                       Type = ApiKey.ApplicationType.Browser,
                       RegexPattern = "pattern",
                       Configuration = ApiKey.ApplicationStatus.Production
                   }));

            var httpContext = new DefaultHttpContext();

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(400);
            } else {
                true.ShouldBe(false);
            }
        }

        [Fact]
        public async Task Should_pass_whitelisted_keys() {
            var ipProvider = new Mock<IServerIpProvider>();

            var keyProvider = new Mock<IBrowserKeyProvider>();
            keyProvider.Setup(x => x.Get(It.IsAny<HttpRequest>()))
                       .Returns("key");

            var apiRepo = new Mock<IApiKeyRepository>();
            apiRepo.Setup(x => x.GetKey(It.IsAny<string>()))
                   .Returns(Task.FromResult(new ApiKey("key") {
                       Whitelisted = true,
                       Deleted = false,
                       Enabled = ApiKey.KeyStatus.Active,
                       Type = ApiKey.ApplicationType.Browser,
                       RegexPattern = "pattern",
                       Configuration = ApiKey.ApplicationStatus.Production
                   }));

            var httpContext = new DefaultHttpContext();

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            contexts.ExecutingContext.Result.ShouldBeNull();
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
                   .Returns(Task.FromResult(new ApiKey("key") {
                       Whitelisted = false,
                       Deleted = false,
                       Enabled = ApiKey.KeyStatus.Active,
                       Type = ApiKey.ApplicationType.Server,
                       Pattern = keyIp,
                       Configuration = ApiKey.ApplicationStatus.Production
                   }));

            var httpContext = new DefaultHttpContext();

            var filter = new AuthorizeApiKeyFromRequest(keyProvider.Object, ipProvider.Object, apiRepo.Object);
            var contexts = CreateContext(httpContext);

            await filter.OnResourceExecutionAsync(contexts.ExecutingContext, () => Task.FromResult(contexts.ExecutedContext));

            if (contexts.ExecutingContext.Result is ObjectResult result) {
                result.StatusCode.ShouldBe(responseCode);
            } else {
                contexts.ExecutingContext.Result.ShouldBe(responseCode);
            }
        }

        private Contexts CreateContext(DefaultHttpContext httpContext) {
            var routeData = new RouteData();
            var actionDescription = new ActionDescriptor();

            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var filterMetadata = new IFilterMetadata[0];
            var valueProvider = new IValueProviderFactory[0];

            var context = new ResourceExecutingContext(actionContext, filterMetadata, valueProvider);
            var resultContext = new ResourceExecutedContext(actionContext, filterMetadata);

            return new Contexts(context, resultContext);
        }

        private class Contexts {
            public Contexts(ResourceExecutingContext c1, ResourceExecutedContext c2)
            {
                ExecutingContext = c1;
                ExecutedContext = c2;
            }

            public ResourceExecutingContext ExecutingContext { get; set; }
            public ResourceExecutedContext ExecutedContext { get; set; }
        }
    }
}
