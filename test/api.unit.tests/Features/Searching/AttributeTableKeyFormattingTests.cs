using ugrc.api.Features.Searching;
using ugrc.api.Infrastructure;
using ugrc.api.Models.Constants;

namespace api.tests.Features.Searching;

public class AttributeTableKeyFormattingTests {
    private readonly IReadOnlyCollection<SearchResponseContract> _data;
    private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _computationHandler;
    private readonly AttributeTableKeyFormatting.Decorator _decorator;

    public AttributeTableKeyFormattingTests() {
        _data = [
            new SearchResponseContract {
                Attributes = new Dictionary<string, object>() {
                    { "UPPER", 0 },
                    { "MixeD", 0 },
                    { "lower", 0 }
                }
            }
        ];

        var mockLogger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

        var dbOptions = Options.Create(new SearchProviderConfiguration());

        var handler = new Mock<IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();
        handler.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data);

        _computationHandler = handler.Object;
        _decorator = new AttributeTableKeyFormatting.Decorator(_computationHandler);
    }

    [Fact]
    public async Task Should_lowercase_all_keys() {
        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        });
        var command = new SqlQuery.Computation("tablename", "UPPER,MixeD,lower", options);

        var result = await _decorator.Handle(command, CancellationToken.None);

        result.First().Attributes.Count.ShouldBe(3);
        result.First().Attributes.Keys.All(x => x.All(char.IsLower)).ShouldBe(true);
    }

    [Fact]
    public async Task Should_uppercase_all_keys() {
        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        });
        var command = new SqlQuery.Computation("tablename", "UPPER,MixeD,lower", options);
        var result = await _decorator.Handle(command, CancellationToken.None);

        result.First().Attributes.Count.ShouldBe(3);
        result.First().Attributes.Keys.All(x => x.All(char.IsUpper)).ShouldBe(true);
    }

    [Fact]
    public async Task Should_keep_all_keys_as_is() {
        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Input
        });
        var command = new SqlQuery.Computation("tablename", "UPPER,MixeD,lower", options);
        var result = await _decorator.Handle(command, CancellationToken.None);

        result.First().Attributes.Count.ShouldBe(3);
        result.ShouldBeSameAs(_data);
    }

    [Fact]
    public async Task Should_handle_null_values_properly() {
        var dataWithNulls = new List<SearchResponseContract> {
            new() {
                Attributes = new Dictionary<string, object>() {
                    { "FIELD1", "value" },
                    { "FIELD2", null! },  // This simulates DBNull converted to null
                    { "FIELD3", 42 }
                }
            }
        };

        var handler = new Mock<IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();
        handler.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataWithNulls);

        var decorator = new AttributeTableKeyFormatting.Decorator(handler.Object);

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        });
        var command = new SqlQuery.Computation("tablename", "FIELD1,FIELD2,FIELD3", options);

        var result = await decorator.Handle(command, CancellationToken.None);

        var attributes = result.First().Attributes;
        attributes.Count.ShouldBe(3);
        attributes["field1"].ShouldBe("value");
        attributes["field2"].ShouldBeNull();  // This should be null, not {}
        attributes["field3"].ShouldBe(42);
    }
}
