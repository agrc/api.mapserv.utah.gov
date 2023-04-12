using AGRC.api.Features.Searching;
using AGRC.api.Models.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace api.tests.Features.Searching;
public class SearchRequestOptionsContractBinderTests {
    private readonly SearchRequestOptionsContractBinder binder;
    public SearchRequestOptionsContractBinderTests() {
        binder = new SearchRequestOptionsContractBinder();
    }

    public static ModelBindingContext GenerateContextFor(string query, string version) {
        var contextMock = new Mock<ModelBindingContext>();
        var action = new Mock<HttpContext>();

        var request = new Mock<HttpRequest>();

        request.SetupProperty(x => x.QueryString, new QueryString(query));
        action.SetupGet(x => x.Request).Returns(request.Object);

        var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, object>() {
            {"version", version}
        }));

        var actionContext = new ActionContext {
            HttpContext = action.Object,
            RouteData = routeData
        };

        contextMock.SetupProperty(x => x.ActionContext, actionContext);
        contextMock.SetupProperty(x => x.Result);

        return contextMock.Object;
    }

    [Fact]
    public async Task Should_bind_defaults_for_version_1() {
        var context = GenerateContextFor(
            "?geometry=point:[1,1]&predicate=1=1&buffer=10",
            "1");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Lower);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_defaults_for_version_2() {
        var context = GenerateContextFor(
            "?geometry=point:[1,1]&predicate=1=1&buffer=10",
            "2");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Input);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_1() {
        var context = GenerateContextFor(
            "?geometry=point:[1,1]&predicate=1=1&buffer=10&attributeStyle=fake",
            "1");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Lower);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_2() {
        var context = GenerateContextFor(
            "?geometry=point:[1,1]&predicate=1=1&buffer=10&attributeStyle=fake",
            "2");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Input);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_all_values() {
        var context = GenerateContextFor(
            "?geometry=point:[1,1]&spatialReference=4326&predicate=1=1&attributeStyle=upper&buffer=10",
            "1");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Upper);
        contract.SpatialReference.ShouldBe(4326);
    }

    [Fact]
    public async Task Should_limit_buffer() {
        var context = GenerateContextFor(
            "?buffer=100000000",
            "1");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.Buffer.ShouldBe(2000);
    }

    [Fact]
    public async Task Should_use_default_for_unknown_spatial_reference() {
        var context = GenerateContextFor(
            "?spatialReference=what",
            "1");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeAssignableTo<SearchRequestOptionsContract>();

        var contract = context.Result.Model as SearchRequestOptionsContract;

        contract.SpatialReference.ShouldBe(26912);
    }
}
