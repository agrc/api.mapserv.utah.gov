using AGRC.api.Features.Geocoding;

namespace api.tests.Features.Geocoding;
public class ReverseGeocodeRequestOptionsContractTests {
    [Fact]
    public async Task Should_bind_defaults_for_version_1() {
        var context = TestHelpers.GenerateContextFor(string.Empty, 1);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Distance.ShouldBe(5);
    }
    [Fact]
    public async Task Should_bind_defaults_for_version_2() {
        var context = TestHelpers.GenerateContextFor(string.Empty, 2);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Distance.ShouldBe(5);
    }
    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_1() {
        var context = TestHelpers.GenerateContextFor(
            "?distance=four&spatialReference=incorrect", 1);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Distance.ShouldBe(5);
    }
    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_2() {
        var context = TestHelpers.GenerateContextFor(
            "?distance=four&spatialReference=incorrect", 2);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Distance.ShouldBe(5);
    }
    [Fact]
    public async Task Should_bind_all_values() {
        var context = TestHelpers.GenerateContextFor(
            "?DISTANCE=500&spatialReference=3857", 1);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(3857);
        contract.Distance.ShouldBe(500);
    }
    [Fact]
    public async Task Should_limit_distance() {
        var context = TestHelpers.GenerateContextFor("?distance=100000000", 1);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.Distance.ShouldBe(2000);
    }
    [Fact]
    public async Task Should_limit_negative_distance() {
        var context = TestHelpers.GenerateContextFor("?distance=-90", 1);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.Distance.ShouldBe(90);
    }
    [Fact]
    public async Task Should_use_default_for_unknown_spatial_reference() {
        var context = TestHelpers.GenerateContextFor("?spatialReference=what", 1);

        var contract = await ReverseGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
    }
}
