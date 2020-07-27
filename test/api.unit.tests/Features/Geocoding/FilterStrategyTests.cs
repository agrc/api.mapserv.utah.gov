using AGRC.api.Features.Geocoding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class FilterStrategyTests {
        [Fact]
        public void Should_return_v1_for_api_v1() {
            var httpContext = Mock.Of<IHttpContextAccessor>(x =>
                x.HttpContext.Request.RouteValues == new RouteValueDictionary(new { version = "1" }));

            var factory = new FilterSuggestionFactory(httpContext);

            factory.GetStrategy(It.IsAny<int>()).ShouldBeOfType<FilterStrategyV1>();
        }

        [Fact]
        public void Should_return_v2_for_api_v2() {
            var httpContext = Mock.Of<IHttpContextAccessor>(x =>
                x.HttpContext.Request.RouteValues == new RouteValueDictionary(new { version = "2" }));

            var factory = new FilterSuggestionFactory(httpContext);

            factory.GetStrategy(It.IsAny<int>()).ShouldBeOfType<FilterStrategyV2>();
        }
    }
}
