using Microsoft.Extensions.Caching.Memory;
using ugrc.api.Features.Geocoding;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;

namespace api.tests.Features.Geocoding;
public class AddressSystemFromZipTests {
    internal static IComputationHandler<AddressSystemFromZipCode.Computation, IReadOnlyCollection<GridLinkable>> _handler;

    public AddressSystemFromZipTests() {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("mapping/zip/1", new List<GridLinkable> { new ZipGridLink(1, "grid", 0) });

        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        _handler = new AddressSystemFromZipCode.Handler(memoryCache, mockLogger.Object);
    }

    [Fact]
    public async Task Should_return_grid_from_zip() {
        var request = new AddressSystemFromZipCode.Computation(1);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.First().Grid.ShouldBe("grid");
    }

    [Fact]
    public async Task Should_return_empty_when_zip_not_found() {
        var request = new AddressSystemFromZipCode.Computation(0);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_return_empty_when_zip_is_null() {
        var request = new AddressSystemFromZipCode.Computation(null);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }
}
