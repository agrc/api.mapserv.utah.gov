using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;

namespace api.tests.Features.Geocoding;
public class AddressSystemFromZipTests {
    internal static IComputationHandler<AddressSystemFromZipCode.Computation, IReadOnlyCollection<GridLinkable>> Handler;

    public AddressSystemFromZipTests() {
        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(x => x.StringGetAsync(It.Is<RedisKey>(p => p.Equals(new RedisKey("1"))), CommandFlags.None))
              .Returns(Task.FromResult(new RedisValue("grid,1")));

        var mockConnection = new Mock<IConnectionMultiplexer>();
        mockConnection.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);

        var redisCache = new RedisCacheRepository(new Lazy<IConnectionMultiplexer>(() => mockConnection.Object));
        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        Handler = new AddressSystemFromZipCode.Handler(redisCache, mockLogger.Object);
    }

    [Fact]
    public async Task Should_return_grid_from_zip() {
        var request = new AddressSystemFromZipCode.Computation(1);
        var result = await Handler.Handle(request, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.First().Grid.ShouldBe("grid");
    }

    [Fact]
    public async Task Should_return_empty_when_zip_not_found() {
        var request = new AddressSystemFromZipCode.Computation(0);
        var result = await Handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_return_empty_when_zip_is_null() {
        var request = new AddressSystemFromZipCode.Computation(null);
        var result = await Handler.Handle(request, CancellationToken.None);

        result.ShouldBeEmpty();
    }
}
