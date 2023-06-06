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

        query.ShouldBe("SELECT field1,field2 FROM table WHERE field1 = 'ok'");
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

        query.ShouldBe("SELECT field1,field2 FROM table WHERE st_intersects(shape,[transformed geometry])");
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

        query.ShouldBe("SELECT field1,field2 FROM table WHERE field1 like 'a%' AND st_intersects(shape,[transformed geometry])");
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

        query.ShouldBe("SELECT field1,field2 FROM table WHERE field1 like 'a%' AND st_intersects(shape,st_buffer([transformed geometry],10))");
    }
}
