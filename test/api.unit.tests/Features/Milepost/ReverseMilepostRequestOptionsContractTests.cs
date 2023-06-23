using AGRC.api.Features.Milepost;

namespace api.tests.Features.Milepost;
public class ReverseMilepostRequestOptionsContractTests {
    [Fact]
    public async Task Should_bind_defaults_for_version_1() {
        var context = TestHelpers.GenerateContextFor(string.Empty, 1);

        var contract = await ReverseRouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(100);
        contract.IncludeRampSystem.ShouldBe(false);
        contract.Suggest.ShouldBe(0);
        contract.SpatialReference.ShouldBe(26912);
    }
    [Fact]
    public async Task Should_bind_defaults_for_version_2() {
        var context = TestHelpers.GenerateContextFor(string.Empty, 2);

        var contract = await ReverseRouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(100);
        contract.IncludeRampSystem.ShouldBe(false);
        contract.Suggest.ShouldBe(0);
        contract.SpatialReference.ShouldBe(26912);
    }
    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_1() {
        var context = TestHelpers.GenerateContextFor(
            "?buffer=unknown&IncludeRAMPSystem=wrong&suggest=four&spatialReference=incorrect", 1);

        var contract = await ReverseRouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(100);
        contract.IncludeRampSystem.ShouldBe(false);
        contract.Suggest.ShouldBe(0);
        contract.SpatialReference.ShouldBe(26912);
    }
    [Fact]
    public async Task Should_bind_all_values() {
        var context = TestHelpers.GenerateContextFor(
            "?includeRampSystem=true&buffer=200&Suggest=3&spatialReference=3857", 1);

        var contract = await ReverseRouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(200);
        contract.IncludeRampSystem.ShouldBe(true);
        contract.Suggest.ShouldBe(3);
        contract.SpatialReference.ShouldBe(3857);
    }
    [Fact]
    public async Task Should_limit_suggest() {
        var context = TestHelpers.GenerateContextFor(
            "?Suggest=10", 1);

        var contract = await ReverseRouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Suggest.ShouldBe(5);
    }
    [Fact]
    public async Task Should_limit_buffer() {
        var context = TestHelpers.GenerateContextFor(
            "?buffer=10000", 1);

        var contract = await ReverseRouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Buffer.ShouldBe(200);
    }
}
