using ugrc.api.Features.Searching;
using ugrc.api.Models.ResponseContracts;

namespace api.tests.Features.Searching;
public class DecodeGeometryDecoratorTests {
    private readonly IApiResponse _data;
    private readonly IRequestHandler<SearchQuery.Query, IApiResponse> _computationHandler;
    private readonly ILogger _logger;
    private SearchQuery.Query _mutation;

    public DecodeGeometryDecoratorTests() {
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
    public async Task Should_decode_legacy_geometry() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = "point:[1,2]",
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._options.Point.ToPostGis().ShouldBe("st_pointfromtext('POINT(1 2)',26912)");
    }

    [Fact]
    public async Task Should_decode_esri_geometry() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = """point:{"x": 1, "y": 2, "spatialReference": { "wkid": 26912}}""",
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._options.Point.ToPostGis().ShouldBe("st_pointfromtext('POINT(1 2)',26912)");
    }

    [Fact]
    public async Task Should_set_esri_spatial_reference() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = """point:{"x": 1, "y": 2, "spatialReference": { "wkid": 26912}}""",
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._options.Point.ToPostGis().ShouldBe("st_pointfromtext('POINT(1 2)',26912)");
    }

    [Fact]
    public async Task Should_use_latest_spatial_reference() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = """point: {"spatialReference":{"latestWkid":26912,"wkid":102100},"x":1,"y":2}""",
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(table);
        _mutation._returnValues.ShouldBe(returnFields);
        _mutation._options.Point.ToPostGis().ShouldBe("st_pointfromtext('POINT(1 2)',26912)");
    }

    [Fact]
    public async Task Should_handle_no_spatial_reference() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = """point: {"x":1,"y":2}""",
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(table);
        _mutation._returnValues.ShouldBe(returnFields);
        _mutation._options.Point.ToPostGis().ShouldBe("st_pointfromtext('POINT(1 2)',26912)");
    }

    [Fact]
    public async Task Should_transform_different_spatial_references() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = """point: {"x":1,"y":2, "spatialReference": { "wkid": 3857 }}""",
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(table);
        _mutation._returnValues.ShouldBe(returnFields);
        _mutation._options.Point.ToPostGis().ShouldBe("st_transform(st_pointfromtext('POINT(1 2)',3857),26912)");
        _mutation._options.Point.ToEsriJson().ShouldBe("""{"x":1,"y":2,"spatialReference":{"wkid":3857}}""");
    }

    [Fact]
    public async Task Should_transform_different_spatial_references_for_legacy() {
        const string table = "table";
        const string returnFields = "field1,field2";

        var options = new SearchOptions(new SearchRequestOptionsContract {
            Geometry = "point:[1,2]",
            SpatialReference = 3857
        });

        var computation = new SearchQuery.Query(table, returnFields, options);
        var decorator = new DecodeGeometryDecorator(_computationHandler, _logger);
        var _ = await decorator.Handle(computation, CancellationToken.None);

        _mutation._tableName.ShouldBe(table);
        _mutation._returnValues.ShouldBe(returnFields);
        _mutation._options.Point.ToPostGis().ShouldBe("st_transform(st_pointfromtext('POINT(1 2)',3857),26912)");
    }
}
