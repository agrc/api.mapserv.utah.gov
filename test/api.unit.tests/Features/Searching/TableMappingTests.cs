using AGRC.api.Features.Searching;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class TableMappingTests {
    private readonly ObjectResult _data;
    private readonly IRequestHandler<SearchQuery.Query, ObjectResult> _computationHandler;
    private readonly ILogger _logger;
    private SearchQuery.Query _mutation;

    public TableMappingTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        _data = new OkObjectResult(string.Empty);

        var handler = new Mock<IRequestHandler<SearchQuery.Query, ObjectResult>>();
        handler.Setup(x => x.Handle(It.IsAny<SearchQuery.Query>(),
                                    It.IsAny<CancellationToken>()))
               .Callback<SearchQuery.Query, CancellationToken>((comp, _) => _mutation = comp)
               .ReturnsAsync(_data);

        _computationHandler = handler.Object;
    }

    [Fact]
    public async Task Should_swap_sgid_table() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        };
        var computation = new SearchQuery.Query("sgid.category.table", "attributes", options);

        var tableMapping = new Mock<ITableMapping>();
        tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "category.table", "swapped" } });

        var decorator = new TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe("swapped");
        _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
        _mutation.Options.Predicate.ShouldBe(computation.Options.Predicate);
        _mutation.Options.AttributeStyle.ShouldBe(computation.Options.AttributeStyle);
    }

    [Fact]
    public async Task Should_skip_non_sgid_tables() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        };
        var computation = new SearchQuery.Query("tablename", "attributes", options);

        var tableMapping = new Mock<ITableMapping>();
        tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "not-found", "value" } });

        var decorator = new TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
        _mutation.Options.Predicate.ShouldBe(computation.Options.Predicate);
        _mutation.Options.AttributeStyle.ShouldBe(computation.Options.AttributeStyle);
    }

    [Fact]
    public async Task Should_throw_if_table_does_not_exist() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        };
        var computation = new SearchQuery.Query("sgid.layer.does-not-exist", "attributes", options);

        var tableMapping = new Mock<ITableMapping>();
        tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "key", "value" } });

        var decorator = new TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None).ShouldThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public void Should_contain_key() {
        var mapping = new TableMapping();
        mapping.MsSqlToPostgres["boundaries.counties"].ShouldBe("boundaries.county_boundaries");
    }
}
