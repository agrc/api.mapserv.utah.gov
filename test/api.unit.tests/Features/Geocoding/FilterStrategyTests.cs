using ugrc.api.Features.Geocoding;

namespace api.tests.Features.Geocoding;
public class FilterStrategyTests {
    [Fact]
    public void Should_return_v1_for_api_v1() {
        var httpContext = TestHelpers.CreateVersionedHttpContext(1);

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

        var factory = new FilterSuggestionFactory(httpContextAccessor.Object);

        factory.GetStrategy(It.IsAny<int>()).ShouldBeOfType<FilterStrategyV1>();
    }

    [Fact]
    public void Should_return_v2_for_api_v2() {
        var httpContext = TestHelpers.CreateVersionedHttpContext(2);

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

        var factory = new FilterSuggestionFactory(httpContextAccessor.Object);

        factory.GetStrategy(It.IsAny<int>()).ShouldBeOfType<FilterStrategyV2>();
    }
}
