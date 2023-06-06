using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class DecodeGeometryDecoratorTests {
    private readonly IReadOnlyCollection<SearchResponseContract> _data;
    private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _computationHandler;
    private readonly ILogger _logger;
    private SqlQuery.Computation _mutation;

    public DecodeGeometryDecoratorTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        _data = new List<SearchResponseContract>{
            new SearchResponseContract()
        };

        var handler = new Mock<IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();
        handler.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(),
                                    It.IsAny<CancellationToken>()))
               .Callback<SqlQuery.Computation, CancellationToken>((comp, _) => _mutation = comp)
               .ReturnsAsync(_data);

        _computationHandler = handler.Object;
    }

    [Fact]
    public async Task Should_decode_legacy_geometry() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = "point:[1,2]",
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.SearchOptions.Geometry.ShouldBe("ST_PointFromText('POINT(1 2)', 26912)");
    }

    [Fact]
    public async Task Should_decode_esri_geometry() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = """point:{"x": 1, "y": 2, "spatialReference": { "wkid": 26912}}""",
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.SearchOptions.Geometry.ShouldBe("ST_PointFromText('POINT(1 2)', 26912)");
    }

    [Fact]
    public async Task Should_set_esri_spatial_reference() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = """point:{"x": 1, "y": 2, "spatialReference": { "wkid": 26912}}""",
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.SearchOptions.Geometry.ShouldBe("ST_PointFromText('POINT(1 2)', 26912)");
    }

    [Fact]
    public async Task Should_use_latest_spatial_reference() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = """point: {"spatialReference":{"latestWkid":26912,"wkid":102100},"x":1,"y":2}""",
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(table);
        _mutation.ReturnValues.ShouldBe(returnFields);
        _mutation.SearchOptions.Geometry.ShouldBe("ST_PointFromText('POINT(1 2)', 26912)");
    }

    [Fact]
    public async Task Should_handle_no_spatial_reference() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = """point: {"x":1,"y":2}""",
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(table);
        _mutation.ReturnValues.ShouldBe(returnFields);
        _mutation.SearchOptions.Geometry.ShouldBe("ST_PointFromText('POINT(1 2)', 26912)");
    }

    [Fact]
    public async Task Should_transform_different_spatial_references() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = """point: {"x":1,"y":2, "spatialReference": { "wkid": 3857 }}""",
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(table);
        _mutation.ReturnValues.ShouldBe(returnFields);
        _mutation.SearchOptions.Geometry.ShouldBe("ST_Transform(ST_PointFromText('POINT(1 2)', 3857), 26912)");
    }

    [Fact]
    public async Task Should_transform_different_spatial_references_for_legacy() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchRequestOptionsContract {
            Geometry = "point:[1,2]",
            SpatialReference = 3857
        };

        var computation = new SqlQuery.Computation(table, returnFields, options);
        var decorator = new SqlQuery.DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation.TableName.ShouldBe(table);
        _mutation.ReturnValues.ShouldBe(returnFields);
        _mutation.SearchOptions.Geometry.ShouldBe("ST_Transform(ST_PointFromText('POINT(1 2)', 3857), 26912)");
    }
}
