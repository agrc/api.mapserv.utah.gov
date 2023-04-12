using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class AttributeTableKeyFormattingTests {
    private readonly IReadOnlyCollection<SearchResponseContract> _data;
    private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _computationHandler;
    private readonly AttributeTableKeyFormatting.Decorator _decorator;

    public AttributeTableKeyFormattingTests() {
        _data = new List<SearchResponseContract>{
            new SearchResponseContract {
                Attributes = new Dictionary<string, object>() {
                    { "UPPER", 0 },
                    { "MixeD", 0 },
                    { "lower", 0 }
                }
            }
        };

        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Array.Empty<SearchResponseContract>() as IReadOnlyCollection<SearchResponseContract>));

        var dbOptions = Options.Create(new SearchProviderConfiguration());

        var handler = new Mock<IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();
        handler.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_data));

        _computationHandler = handler.Object;
        _decorator = new AttributeTableKeyFormatting.Decorator(_computationHandler);
    }

    [Fact]
    public async Task Should_lowercase_all_keys() {
        var command = new SqlQuery.Computation("tablename", "attributes", "query", AttributeStyle.Lower);

        var result = await _decorator.Handle(command, CancellationToken.None);

        result.First().Attributes.Count.ShouldBe(3);
        result.First().Attributes.Keys.All(x => x.All(char.IsLower)).ShouldBe(true);
    }

    [Fact]
    public async Task Should_uppercase_all_keys() {
        var command = new SqlQuery.Computation("tablename", "attributes", "query", AttributeStyle.Upper);

        var result = await _decorator.Handle(command, CancellationToken.None);

        result.First().Attributes.Count.ShouldBe(3);
        result.First().Attributes.Keys.All(x => x.All(char.IsUpper)).ShouldBe(true);
    }

    [Fact]
    public async Task Should_keep_all_keys_as_is() {
        var command = new SqlQuery.Computation("tablename", "attributes", "query", AttributeStyle.Input);
        var result = await _decorator.Handle(command, CancellationToken.None);

        result.First().Attributes.Count.ShouldBe(3);
        result.ShouldBeSameAs(_data);
    }
}
