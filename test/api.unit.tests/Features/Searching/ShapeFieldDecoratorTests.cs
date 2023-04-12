using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class ShapeFieldDecoratorTests {
    private readonly IReadOnlyCollection<SearchResponseContract> _data;
    private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _computationHandler;
    private readonly ILogger _logger;
    private SqlQuery.Computation _mutation;

    public ShapeFieldDecoratorTests() {
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
    public async Task Should_modify_shape_token() {
        var computation = new SqlQuery.Computation("table", "shape@,field2", "query", AttributeStyle.Lower);

        var decorator = new SqlQuery.ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe("st_simplify(shape,10) as shape,field2");
        _mutation.Predicate.ShouldBe(computation.Predicate);
        _mutation.Styling.ShouldBe(computation.Styling);
    }

    [Fact]
    public async Task Should_modify_envelope_token() {
        var computation = new SqlQuery.Computation("table", "shape@envelope,field2", "query", AttributeStyle.Lower);

        var decorator = new SqlQuery.ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe("st_envelope(shape) as shape,field2");
        _mutation.Predicate.ShouldBe(computation.Predicate);
        _mutation.Styling.ShouldBe(computation.Styling);
    }

    [Fact]
    public async Task Should_skip_non_spatial_fields() {
        var computation = new SqlQuery.Computation("table", "envelope,field2", "query", AttributeStyle.Upper);

        var decorator = new SqlQuery.ShapeFieldDecorator(_computationHandler, _logger);

        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(computation.TableName);
        _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
        _mutation.Predicate.ShouldBe(computation.Predicate);
        _mutation.Styling.ShouldBe(computation.Styling);
    }
}
