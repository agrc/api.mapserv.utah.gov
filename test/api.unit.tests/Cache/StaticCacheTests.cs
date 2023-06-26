using AGRC.api.Cache;

namespace api.tests.Cache;

public class StaticCacheTests {
    internal readonly StaticCache _cache;

    public StaticCacheTests() {
        _cache = new StaticCache();
    }

    [Fact]
    public void Should_create_distinct_list_of_po_box_exclusions() {
        _cache.PoBoxExclusions.ShouldBeUnique();
        _cache.PoBoxes.ShouldBeUnique();
        _cache.PoBoxZipCodesWithExclusions.ShouldBeUnique();
    }
    [Fact]
    public void Should_find_exclusion_from_zip_plus_po_box() {
        const int zipCode = 84716;
        const int poBox = 1313;

        const int key = (zipCode * 10000) + poBox;

        _cache.PoBoxExclusions.ContainsKey(key).ShouldBeTrue();
        _cache.PoBoxes.ContainsKey(zipCode).ShouldBeTrue();

        var exclusion = _cache.PoBoxExclusions[key];
        exclusion.X.ShouldBe(462787.3238216745d);
        exclusion.Y.ShouldBe(4195803.554941956d);
    }
    [Fact]
    public void Should_provide_po_box_location_when_no_exclusion() {
        const int zipCode = 84716;
        const int poBox = 0;

        _cache.PoBoxExclusions.ContainsKey((zipCode * 10000) + poBox).ShouldBeFalse();
        _cache.PoBoxes.ContainsKey(zipCode).ShouldBeTrue();

        var poBoxAddress = _cache.PoBoxes[zipCode];
        poBoxAddress.X.ShouldBe(462787.20);
        poBoxAddress.Y.ShouldBe(4195748.36);
    }
}
