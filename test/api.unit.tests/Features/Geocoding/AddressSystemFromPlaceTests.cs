using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;

namespace api.tests.Features.Geocoding;
public class AddressSystemFromPlaceTests {
    public AddressSystemFromPlaceTests() {
        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(x => x.StringGetAsync(It.Is<RedisKey>(p => p.Equals(new RedisKey("place"))), CommandFlags.None))
              .Returns(Task.FromResult(new RedisValue("grid,0")));

        var mockConnection = new Mock<IConnectionMultiplexer>();
        mockConnection.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);

        var redisCache = new RedisCacheRepository(new Lazy<IConnectionMultiplexer>(() => mockConnection.Object));
        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        Handler = new AddressSystemFromPlace.Handler(redisCache, mockLogger.Object);
    }

    internal static IComputationHandler<AddressSystemFromPlace.Computation, IReadOnlyCollection<GridLinkable>> Handler;

    [Fact]
    public async Task Should_return_empty_when_zip_is_null() {
        var place = string.Empty;
        var request = new AddressSystemFromPlace.Computation(place);
        var result = await Handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_return_empty_when_zip_not_found() {
        var request = new AddressSystemFromPlace.Computation("other place");
        var result = await Handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_return_grid_from_place() {
        var request = new AddressSystemFromPlace.Computation("place");
        var result = await Handler.Handle(request, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.First().Grid.ShouldBe("grid");
    }
}
