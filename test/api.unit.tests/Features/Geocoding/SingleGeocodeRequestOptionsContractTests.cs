using AGRC.api.Features.Geocoding;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Geocoding;
public class SingleGeocodeRequestOptionsContractTests {
    [Fact]
    public async Task Should_bind_defaults_for_version_1() {
        var context = HttpContextHelpers.GenerateContextFor(string.Empty, 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
        contract.AcceptScore.ShouldBe(70);
        contract.Suggest.ShouldBe(0);
        contract.Locators.ShouldBe(LocatorType.All);
        contract.PoBox.ShouldBe(false);
        contract.ScoreDifference.ShouldBe(false);
    }
    [Fact]
    public async Task Should_bind_defaults_for_version_2() {
        var context = HttpContextHelpers.GenerateContextFor(string.Empty, 2);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
        contract.AcceptScore.ShouldBe(70);
        contract.Suggest.ShouldBe(0);
        contract.Locators.ShouldBe(LocatorType.All);
        contract.PoBox.ShouldBe(true);
        contract.ScoreDifference.ShouldBe(false);
    }
    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_1() {
        var context = HttpContextHelpers.GenerateContextFor(
            "?format=unknown&locators=wrong&buffer=four&spatialReference=incorrect&suggest=ok&scoreDifference=boo&pobox=wrong", 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
        contract.AcceptScore.ShouldBe(70);
        contract.Suggest.ShouldBe(0);
        contract.Locators.ShouldBe(LocatorType.All);
        contract.PoBox.ShouldBe(false);
        contract.ScoreDifference.ShouldBe(false);
    }
    [Fact]
    public async Task Should_bind_defaults_for_unknown_for_version_2() {
        var context = HttpContextHelpers.GenerateContextFor(
            "?format=unknown&locators=wrong&buffer=four&spatialReference=incorrect&suggest=ok&scoreDifference=boo&pobox=wrong",
            2);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
        contract.Format.ShouldBe(JsonFormat.None);
        contract.AcceptScore.ShouldBe(70);
        contract.Suggest.ShouldBe(0);
        contract.Locators.ShouldBe(LocatorType.All);
        contract.PoBox.ShouldBe(true);
        contract.ScoreDifference.ShouldBe(false);
    }
    [Fact]
    public async Task Should_bind_all_values() {
        var context = HttpContextHelpers.GenerateContextFor(
            "?format=eSRIJson&locators=RoADCenterLines&acceptScore=10&spatialReference=3857&suggest=3&pobox=true",
            1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(3857);
        contract.Format.ShouldBe(JsonFormat.EsriJson);
        contract.AcceptScore.ShouldBe(10);
        contract.Suggest.ShouldBe(3);
        contract.Locators.ShouldBe(LocatorType.RoadCenterlines);
        contract.PoBox.ShouldBe(true);
        contract.ScoreDifference.ShouldBe(false);
    }
    [Fact]
    public async Task Should_limit_accept_score() {
        var context = HttpContextHelpers.GenerateContextFor("?acceptScore=100000000", 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.AcceptScore.ShouldBe(100);
    }
    [Fact]
    public async Task Should_limit_negative_accept_score() {
        var context = HttpContextHelpers.GenerateContextFor("?acceptScore=-90", 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.AcceptScore.ShouldBe(90);
    }
    [Fact]
    public async Task Should_limit_suggest() {
        var context = HttpContextHelpers.GenerateContextFor("?suggest=100000000", 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.Suggest.ShouldBe(5);
    }
    [Fact]
    public async Task Should_limit_negative_suggest() {
        var context = HttpContextHelpers.GenerateContextFor("?suggest=-20", 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.Suggest.ShouldBe(5);
    }
    [Fact]
    public async Task Should_use_default_for_unknown_spatial_reference() {
        var context = HttpContextHelpers.GenerateContextFor("?spatialReference=what", 1);

        var contract = await SingleGeocodeRequestOptionsContract.BindAsync(context);

        contract.SpatialReference.ShouldBe(26912);
    }
}
