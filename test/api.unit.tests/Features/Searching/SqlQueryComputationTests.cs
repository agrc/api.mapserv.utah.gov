using AGRC.api.Features.Searching;
using AGRC.api.Models.Constants;

namespace api.tests.Features.Searching;
public class SqlQueryComputationTests {
    private const string Table = "table";
    [Fact]
    public void Should_build_attribute_query() {
        const string returnFields = "field1,field2";
        var options = new SearchRequestOptionsContract {
            Predicate = string.Empty,
            AttributeStyle = AttributeStyle.Lower
        };

        var computation = new SqlQuery.Computation(Table, returnFields, options);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table");
    }

    [Fact]
    public void Should_build_attribute_query_with_predicate() {
        const string returnFields = "field1,field2";
        var options = new SearchRequestOptionsContract {
            Predicate = "field1 = 'ok'",
            AttributeStyle = AttributeStyle.Lower
        };

        var computation = new SqlQuery.Computation(Table, returnFields, options);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE FIELD1 = 'OK'");
    }

    [Fact]
    public void Should_add_geometry_to_where() {
        const string returnFields = "field1,field2";
        var options = new SearchRequestOptionsContract {
            Predicate = string.Empty,
            AttributeStyle = AttributeStyle.Lower,
            Geometry = "[transformed geometry]"
        };

        var computation = new SqlQuery.Computation(Table, returnFields, options);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE ST_Intersects(Shape,[transformed geometry])");
    }

    [Fact]
    public void Should_join_geometry_and_predicate_to_where() {
        const string returnFields = "field1,field2";
        var options = new SearchRequestOptionsContract {
            Predicate = "field1 like 'a%'",
            Geometry = "[transformed geometry]",
            AttributeStyle = AttributeStyle.Lower
        };

        var computation = new SqlQuery.Computation(Table, returnFields, options);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE FIELD1 LIKE 'A%' AND ST_Intersects(Shape,[transformed geometry])");
    }

    [Fact]
    public void Should_buffer_geometry() {
        const string returnFields = "field1,field2";
        var options = new SearchRequestOptionsContract {
            Predicate = "field1 like 'a%'",
            Geometry = "[transformed geometry]",
            AttributeStyle = AttributeStyle.Lower,
            Buffer = 10,
        };

        var computation = new SqlQuery.Computation(Table, returnFields, options);
        var query = computation.BuildQuery();

        query.ShouldBe("SELECT field1,field2 FROM table WHERE FIELD1 LIKE 'A%' AND ST_Intersects(Shape,ST_Buffer([transformed geometry],10))");
    }
}
