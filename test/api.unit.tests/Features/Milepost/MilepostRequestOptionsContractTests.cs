using ugrc.api.Features.Milepost;
using ugrc.api.Models.Constants;

namespace api.tests.Features.Milepost;
public class MilepostRequestOptionsContractTests {
    [Fact]
    public async Task Should_bind_defaults_for_version_1() {
        var context = TestHelpers.GenerateContextFor(string.Empty, 1);

        var contract = await RouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Side.ShouldBe(SideDelineation.Increasing);
        contract.FullRoute.ShouldBe(false);
        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
    }
    [Fact]
    public async Task Should_bind_defaults_for_version_2() {
        var context = TestHelpers.GenerateContextFor(string.Empty, 2);

        var contract = await RouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Side.ShouldBe(SideDelineation.Increasing);
        contract.FullRoute.ShouldBe(false);
        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
    }
    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_1() {
        var context = TestHelpers.GenerateContextFor(
            "?format=unknown&SIDE=wrong&fullrouTe=four&spatialReference=incorrect", 1);

        var contract = await RouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Side.ShouldBe(SideDelineation.Increasing);
        contract.FullRoute.ShouldBe(false);
        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
    }
    [Fact]
    public async Task Should_bind_all_values() {
        var context = TestHelpers.GenerateContextFor(
            "?format=GEOJSON&SIDE=decreasING&fullrouTe=true&spatialReference=3857", 1);

        var contract = await RouteMilepostRequestOptionsContract.BindAsync(context);

        contract.Side.ShouldBe(SideDelineation.Decreasing);
        contract.FullRoute.ShouldBe(true);
        contract.SpatialReference.ShouldBe(3857);
        contract.Format.ShouldBe(JsonFormat.GeoJson);
    }
}
