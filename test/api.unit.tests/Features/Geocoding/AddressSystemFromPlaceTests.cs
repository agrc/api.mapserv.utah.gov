using ugrc.api.Cache;
using ugrc.api.Features.Geocoding;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Linkables;

namespace api.tests.Features.Geocoding;
public class AddressSystemFromPlaceTests {
    public AddressSystemFromPlaceTests() {
        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(x => x.StringGetAsync(It.Is<RedisKey>(p => p.Equals(new RedisKey("map/place/key"))), CommandFlags.None))
              .ReturnsAsync(new RedisValue("grid,0"));

        var mockConnection = new Mock<IConnectionMultiplexer>();
        mockConnection.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);

        var redisCache = new RedisCacheRepository(new Lazy<IConnectionMultiplexer>(() => mockConnection.Object));
        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        _handler = new AddressSystemFromPlace.Handler(redisCache, mockLogger.Object);
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
