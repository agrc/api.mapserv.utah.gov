using Microsoft.Extensions.Caching.Memory;
using ugrc.api.Features.Geocoding;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;
using ZiggyCreatures.Caching.Fusion;

namespace api.tests.Features.Geocoding;
public class AddressSystemFromPlaceTests {
    public AddressSystemFromPlaceTests() {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("mapping/place/key", new List<GridLinkable> { new PlaceGridLink("key", "grid", 0) });

        var mockFusionCache = new Mock<IFusionCache>();
        var mockFusionCacheProvider = new Mock<IFusionCacheProvider>();
        mockFusionCacheProvider.Setup(x => x.GetCache(It.IsAny<string>())).Returns(mockFusionCache.Object);
        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        _handler = new AddressSystemFromPlace.Handler(memoryCache, mockFusionCacheProvider.Object, mockLogger.Object);
    }

    internal static IComputationHandler<AddressSystemFromPlace.Computation, IReadOnlyCollection<GridLinkable>> _handler;

    [Fact]
    public async Task Should_return_empty_when_zip_is_null() {
        var place = string.Empty;
        var request = new AddressSystemFromPlace.Computation(place);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_return_empty_when_zip_not_found() {
        var request = new AddressSystemFromPlace.Computation("key-not-found");
        var result = await _handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_return_grid_from_place() {
        var request = new AddressSystemFromPlace.Computation("key");
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.First().Grid.ShouldBe("grid");
    }

    private static (AddressSystemFromPlace.Handler handler, IFusionCache places, MemoryCache placesMemory) CreateHandlerWithPlacesCache(long sizeLimit = 10_000) {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("mapping/place/key", new List<GridLinkable> { new PlaceGridLink("key", "grid", 0) });
        memoryCache.Set("mapping/places", new List<string> { "key" });

        // mirrors the places cache registration: in process only and size limited
        var placesMemory = new MemoryCache(new MemoryCacheOptions { SizeLimit = sizeLimit, TrackStatistics = true });
        var places = new FusionCache(new FusionCacheOptions {
            CacheKeyPrefix = "places/",
            DefaultEntryOptions = new FusionCacheEntryOptions {
                IsFailSafeEnabled = true,
                Duration = TimeSpan.FromDays(7),
                FailSafeMaxDuration = TimeSpan.FromDays(8),
                Size = 1,
            }
        }, placesMemory);

        var mockFusionCacheProvider = new Mock<IFusionCacheProvider>();
        mockFusionCacheProvider.Setup(x => x.GetCache(It.IsAny<string>())).Returns(places);
        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        return (new AddressSystemFromPlace.Handler(memoryCache, mockFusionCacheProvider.Object, mockLogger.Object), places, placesMemory);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("keyxyz")]
    [InlineData("key-not-found")]
    public async Task Should_not_cache_zones_that_do_not_match_a_place(string zone) {
        var (handler, places, _) = CreateHandlerWithPlacesCache();

        var result = await handler.Handle(new AddressSystemFromPlace.Computation(zone), CancellationToken.None);

        result.ShouldBeEmpty();
        (await places.TryGetAsync<List<GridLinkable>>($"mapping/place/{zone}")).HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_not_match_or_cache_a_very_long_zone() {
        var (handler, places, _) = CreateHandlerWithPlacesCache();
        var zone = new string('k', 8_000);

        var result = await handler.Handle(new AddressSystemFromPlace.Computation(zone), CancellationToken.None);

        result.ShouldBeEmpty();
        (await places.TryGetAsync<List<GridLinkable>>($"mapping/place/{zone}")).HasValue.ShouldBeFalse();
    }

    [Theory]
    [InlineData("kee")]
    [InlineData("ky")]
    [InlineData("keyxy")]
    [InlineData("kexyz")]
    public async Task Should_return_and_cache_grid_from_a_place_within_two_edits(string zone) {
        var (handler, places, _) = CreateHandlerWithPlacesCache();

        var result = await handler.Handle(new AddressSystemFromPlace.Computation(zone), CancellationToken.None);

        result.Count.ShouldBe(1);
        result.First().Grid.ShouldBe("grid");
        (await places.TryGetAsync<List<GridLinkable>>($"mapping/place/{zone}")).HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_keep_the_places_cache_within_its_size_limit() {
        var (handler, _, placesMemory) = CreateHandlerWithPlacesCache(sizeLimit: 2);

        foreach (var zone in new[] { "kea", "keb", "kec", "ked", "kee" }) {
            var result = await handler.Handle(new AddressSystemFromPlace.Computation(zone), CancellationToken.None);

            result.Count.ShouldBe(1);
            result.First().Grid.ShouldBe("grid");
        }

        placesMemory.GetCurrentStatistics()!.CurrentEstimatedSize!.Value.ShouldBeLessThanOrEqualTo(2);
    }
}
