using AGRC.api.Features.Searching;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class ShapeFieldDecoratorTests {
    private readonly ObjectResult _data;
    private readonly IRequestHandler<SearchQuery.Query, ObjectResult> _computationHandler;
    private readonly ILogger _logger;
    private SearchQuery.Query _mutation;

    public ShapeFieldDecoratorTests() {
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
    public async Task Should_modify_shape_token() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        };
        var computation = new SearchQuery.Query("table", "shape@,field2", options);

        var decorator = new ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe("st_simplify(shape,10) as shape,field2");
        _mutation.Options.Predicate.ShouldBe(computation.Options.Predicate);
        _mutation.Options.AttributeStyle.ShouldBe(computation.Options.AttributeStyle);
    }

    [Fact]
    public async Task Should_modify_envelope_token() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Lower
        };
        var computation = new SearchQuery.Query("table", "shape@envelope,field2", options);

        var decorator = new ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe("st_envelope(shape) as shape,field2");
        _mutation.Options.Predicate.ShouldBe(computation.Options.Predicate);
        _mutation.Options.AttributeStyle.ShouldBe(computation.Options.AttributeStyle);
    }

    [Fact]
    public async Task Should_skip_non_spatial_fields() {
        var options = new SearchRequestOptionsContract {
            Predicate = "query",
            AttributeStyle = AttributeStyle.Upper
        };
        var computation = new SearchQuery.Query("table", "envelope,field2", options);

        var decorator = new ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
        _mutation.Options.Predicate.ShouldBe(computation.Options.Predicate);
        _mutation.Options.AttributeStyle.ShouldBe(computation.Options.AttributeStyle);
    }
}
