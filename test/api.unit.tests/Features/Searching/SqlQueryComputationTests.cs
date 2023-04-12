using AGRC.api.Features.Searching;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class SqlQueryComputationTests {
    private const string table = "table";
    [Fact]
    public void Should_build_attribute_query() {
        const string returnFields = "field1,field2";
        var predicate = string.Empty;

        var computation = new SqlQuery.Computation(table, returnFields, predicate, AttributeStyle.Lower);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table");
    }

    [Fact]
    public void Should_build_attribute_query_with_predicate() {
        const string returnFields = "field1,field2";
        const string predicate = "field1 = 'ok'";

        var computation = new SqlQuery.Computation(table, returnFields, predicate, AttributeStyle.Lower);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE field1 = 'ok'");
    }

    [Fact]
    public void Should_add_legacy_geometry_to_where() {
        const string returnFields = "field1,field2";
        var predicate = string.Empty;
        const string geometry = "point:[1,1]";

        var computation = new SqlQuery.Computation(table, returnFields, predicate, AttributeStyle.Lower, geometry);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE ST_Intersects(Shape, ST_PointFromText('POINT(1 1)', 26912))");
    }

    [Fact]
    public void Should_add_esri_geometry_to_where() {
        const string returnFields = "field1,field2";
        var predicate = string.Empty;
        const string geometry = "point:{\"x\" : 1, \"y\" : 1, \"spatialReference\" : { \"wkid\": 26912}}";

        var computation = new SqlQuery.Computation(table, returnFields, predicate, AttributeStyle.Lower, geometry);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE ST_Intersects(Shape, ST_PointFromText('POINT(1 1)', 26912))");
    }

    [Fact]
    public void Should_join_geometry_and_predicate_to_where() {
        const string returnFields = "field1,field2";
        const string predicate = "field1 like 'a%'";
        const string geometry = "point:[1,1]";

        var computation = new SqlQuery.Computation(table, returnFields, predicate, AttributeStyle.Lower, geometry);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE field1 like 'a%' AND ST_Intersects(Shape, ST_PointFromText('POINT(1 1)', 26912))");
    }
}
