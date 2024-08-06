using ugrc.api.Features.Searching;
using ugrc.api.Models.Constants;
using ugrc.api.Models.ResponseContracts;

namespace api.tests.Features.Searching;
public class ShapeFieldDecoratorTests {
    private readonly IApiResponse _data;
    private readonly IRequestHandler<SearchQuery.Query, IApiResponse> _computationHandler;
    private readonly ILogger _logger;
    private SearchQuery.Query _mutation;

    public ShapeFieldDecoratorTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        _data = new ApiResponseContract();

        var handler = new Mock<IRequestHandler<SearchQuery.Query, IApiResponse>>();
        handler.Setup(x => x.Handle(It.IsAny<SearchQuery.Query>(),
                                    It.IsAny<CancellationToken>()))
               .Callback<SearchQuery.Query, CancellationToken>((comp, _) => _mutation = comp)
               .ReturnsAsync(_data);

        _computationHandler = handler.Object;
    }

    [Fact]
    public async Task Should_modify_shape_token() {
        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        });
        var computation = new SearchQuery.Query("table", "shape@,field2", options);

        var decorator = new ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(computation._tableName);
        _mutation._returnValues.ShouldBe("st_simplify(shape,10,true) as shape,field2");
        _mutation._options.Predicate.ShouldBe(computation._options.Predicate);
        _mutation._options.AttributeStyle.ShouldBe(computation._options.AttributeStyle);
    }

    [Fact]
    public async Task Should_modify_envelope_token() {
        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        });
        var computation = new SearchQuery.Query("table", "shape@envelope,field2", options);

        var decorator = new ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(computation._tableName);
        _mutation._returnValues.ShouldBe("st_envelope(shape) as shape,field2");
        _mutation._options.Predicate.ShouldBe(computation._options.Predicate);
        _mutation._options.AttributeStyle.ShouldBe(computation._options.AttributeStyle);
    }

    [Fact]
    public async Task Should_skip_non_spatial_fields() {
        var options = new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        });
        var computation = new SearchQuery.Query("table", "envelope,field2", options);

        var decorator = new ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(computation._tableName);
        _mutation._returnValues.ShouldBe(computation._returnValues);
        _mutation._options.Predicate.ShouldBe(computation._options.Predicate);
        _mutation._options.AttributeStyle.ShouldBe(computation._options.AttributeStyle);
    }
}
