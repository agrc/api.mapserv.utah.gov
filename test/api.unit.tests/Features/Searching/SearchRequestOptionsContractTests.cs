using AGRC.api.Features.Searching;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class SearchRequestOptionsContractTests {
    [Fact]
    public async Task Should_bind_defaults_for_version_1() {
        var context = TestHelpers.GenerateContextFor("?geometry=point:[1,1]&predicate=1=1&buffer=10", 1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Lower);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_defaults_for_version_2() {
        var context = TestHelpers.GenerateContextFor("?geometry=point:[1,1]&predicate=1=1&buffer=10", 2);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Input);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_1() {
        var context = TestHelpers.GenerateContextFor(
            "?geometry=point:[1,1]&predicate=1=1&buffer=10&attributeStyle=fake",
            1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Lower);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_2() {
        var context = TestHelpers.GenerateContextFor(
            "?geometry=point:[1,1]&predicate=1=1&buffer=10&attributeStyle=fake",
            2);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Input);
        contract.SpatialReference.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_bind_all_values() {
        var context = TestHelpers.GenerateContextFor(
            "?geometry=point:[1,1]&spatialReference=4326&predicate=1=1&attributeStyle=upper&buffer=10",
            1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Predicate.ShouldBe("1=1");
        contract.Geometry.ShouldBe("point:[1,1]");
        contract.Buffer.ShouldBe(10);
        contract.AttributeStyle.ShouldBe(AttributeStyle.Upper);
        contract.SpatialReference.ShouldBe(4326);
    }

    [Fact]
    public async Task Should_limit_buffer() {
        var context = TestHelpers.GenerateContextFor("?buffer=100000000", 1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(2000);
    }

    [Fact]
    public async Task Should_use_default_for_unknown_spatial_reference() {
        var context = TestHelpers.GenerateContextFor("?spatialReference=what", 1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
    }
    [Fact]
    public async Task Should_handle_negative_values() {
        var context = TestHelpers.GenerateContextFor("?buffer=-5", 1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(5);
    }
    [Fact]
    public async Task Should_set_max_values_values() {
        var context = TestHelpers.GenerateContextFor("?buffer=-5000", 1);

        var contract = await SearchRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(2000);
    }
}
