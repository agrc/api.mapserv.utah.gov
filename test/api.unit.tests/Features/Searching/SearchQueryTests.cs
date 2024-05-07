using ugrc.api.Features.Searching;
using ugrc.api.Infrastructure;
using ugrc.api.Models.ResponseContracts;
using EsriJson.Net;

namespace api.tests.Features.Searching;
public class SearchQueryTests {
    private readonly ILogger _logger;
    public SearchQueryTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
    }

    [Fact]
    public async Task Should_return_results() {
        var query = new SearchQuery.Query("tableName", "return,values", new(new()));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] {
                    new SearchResponseContract {
                        Geometry = new SerializableGraphic(
                            new Graphic(
                                new EsriJson.Net.Geometry.Point(1, 1),
                                new Dictionary<string, object>()
                            )),
                        Attributes = new Dictionary<string, object> {
                            { "key", "value"}
                        }
                    }
                });

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None) as IApiResponse<IReadOnlyCollection<SearchResponseContract>>;

        result.Status.ShouldBe(200);
        result.Result.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Should_handle_sgid_table_not_found() {
        var query = new SearchQuery.Query("table_not_found", "return,values", new(new()));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException());

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(400);
        result.Result.ShouldBeAssignableTo<IApiResponse>();
        result.Message.ShouldBe("The table `table_not_found` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/sgid/#open-sgid) to verify the table exists. Please read https://gis.utah.gov/sgid-product-relaunch-update/#static-sgid-data-layers for more information.");
    }

    [Fact]
    public async Task Should_handle_pg_table_not_found() {
        var query = new SearchQuery.Query("table_not_found", "return,values", new(new()));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new PostgresException("""relation "table_not_found" does not exist""", "ERROR", "ERROR", "42P01"));

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(400);
        result.Result.ShouldBeAssignableTo<IApiResponse>();
        result.Message.ShouldBe("The table `table_not_found` does not exist in the Open SGID.");
    }

    [Fact]
    public async Task Should_handle_invalid_columns() {
        var query = new SearchQuery.Query("table_not_found", "not_found", new(new()));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new PostgresException("""column "not_found" does not exist""", "ERROR", "ERROR", "42703"));

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(400);
        result.Result.ShouldBeAssignableTo<IApiResponse>();
        result.Message.ShouldBe("column `not_found` does not exist on `table_not_found`. Check that the fields exist.");
    }

    [Fact]
    public async Task Should_handle_invalid_predicate() {
        var query = new SearchQuery.Query("table", "field", new(new SearchRequestOptionsContract {
            Predicate = "bad predicate"
        }));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new PostgresException("argument of WHERE must be type boolean, not type character varying", "ERROR", "ERROR", "42804"));

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(400);
        result.Result.ShouldBeAssignableTo<IApiResponse>();
        result.Message.ShouldBe("`bad predicate` is not a valid T-SQL where clause.");
    }

    [Fact]
    public async Task Should_handle_unhandled_pg_exception() {
        var query = new SearchQuery.Query("table", "field", new(new SearchRequestOptionsContract {
            Predicate = "bad predicate"
        }));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new PostgresException("unknown", "ERROR", "ERROR", "unknown"));

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(400);
        result.Result.ShouldBeAssignableTo<IApiResponse>();
        result.Message.ShouldBe("unknown");
    }

    [Fact]
    public async Task Should_handle_unhandled_exception() {
        var query = new SearchQuery.Query("table", "field", new(new SearchRequestOptionsContract {
            Predicate = "bad predicate"
        }));

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("unknown"));

        var handler = new SearchQuery.Handler(mediator.Object, _logger);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(400);
        result.Result.ShouldBeAssignableTo<IApiResponse>();
        result.Message.ShouldBe("The table `table` might not exist. Check your spelling.");
    }
}
