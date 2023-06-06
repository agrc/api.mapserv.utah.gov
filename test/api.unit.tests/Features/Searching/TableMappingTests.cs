using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class TableMappingTests {
    private readonly IReadOnlyCollection<SearchResponseContract> _data;
    private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _computationHandler;
    private readonly ILogger _logger;
    private SqlQuery.Computation _mutation;

    public TableMappingTests() {
        _data = new List<SearchResponseContract>{
            new SearchResponseContract {
                Attributes = new Dictionary<string, object>() {
                    { "UPPER", 0 },
                    { "MixeD", 0 },
                    { "lower", 0 }
                }
            }
        };

        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        var handler = new Mock<IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();
        handler.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(),
                                    It.IsAny<CancellationToken>()))
               .Callback<SqlQuery.Computation, CancellationToken>((comp, _) => _mutation = comp)
               .ReturnsAsync(_data);
        _computationHandler = handler.Object;
    }

    [Fact]
    public async Task Should_swap_sgid_table() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        };
        var computation = new SqlQuery.Computation("sgid.category.table", "attributes", options);

        var tableMapping = new Mock<ITableMapping>();
        tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "category.table", "swapped" } });

        var decorator = new SqlQuery.TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe("swapped");
        _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
        _mutation.SearchOptions.Predicate.ShouldBe(computation.SearchOptions.Predicate);
        _mutation.SearchOptions.AttributeStyle.ShouldBe(computation.SearchOptions.AttributeStyle);
    }

    [Fact]
    public async Task Should_skip_non_sgid_tables() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        };
        var computation = new SqlQuery.Computation("tablename", "attributes", options);

        var tableMapping = new Mock<ITableMapping>();
        tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "not-found", "value" } });

        var decorator = new SqlQuery.TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
        _mutation.SearchOptions.Predicate.ShouldBe(computation.SearchOptions.Predicate);
        _mutation.SearchOptions.AttributeStyle.ShouldBe(computation.SearchOptions.AttributeStyle);
    }

    [Fact]
    public async Task Should_throw_if_table_does_not_exist() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        };
        var computation = new SqlQuery.Computation("sgid.layer.does-not-exist", "attributes", options);

        var tableMapping = new Mock<ITableMapping>();
        tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "key", "value" } });

        var decorator = new SqlQuery.TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None).ShouldThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public void Should_contain_key() {
        var mapping = new TableMapping();
        mapping.MsSqlToPostgres["boundaries.counties"].ShouldBe("boundaries.county_boundaries");
    }
}
