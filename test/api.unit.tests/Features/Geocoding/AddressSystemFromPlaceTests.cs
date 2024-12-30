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
}
