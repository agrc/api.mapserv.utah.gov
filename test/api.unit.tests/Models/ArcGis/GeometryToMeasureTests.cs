using AGRC.api.Models;
using AGRC.api.Models.ArcGis;

namespace api.tests.Models.ArcGis;

public class GeometryToMeasureTests {
    [Fact]
    public void Should_create_valid_query_string() {
        // create new GeometryToMeasureTests.RequestContract with a RequestLocation
        var model = new GeometryToMeasure.RequestContract {
            Locations = new[] {
                new GeometryToMeasure.RequestLocation(new Point(1, 2))
            },
            Tolerance = 1,
            OutSr = 102100
        };

        // ?f=json&locations=[{"geometry":{"x":1,"y":2}}]&outSR=102100&inSR=26912&tolerance=1
        model.QueryString.ShouldBe("?f=json&locations=%5B%7B%22geometry%22%3A%7B%22x%22%3A1,%22y%22%3A2%7D%7D%5D&outSR=102100&inSR=26912&tolerance=1");
    }
    [Fact]
    public void Should_create_valid_query_string_with_multiple_locations() {
        // create new GeometryToMeasure.RequestContract with a RequestLocation
        var model = new GeometryToMeasure.RequestContract {
            Locations = new[] {
               new GeometryToMeasure.RequestLocation(new Point(1, 2)),
               new GeometryToMeasure.RequestLocation(new Point(3, 4))
            },
            Tolerance = 1
        };

        // ?f=json&locations={"geometry":{"x":1,"y":2}}&outSR=102100&inSR=26912&tolerance=1
        model.QueryString.ShouldBe("?f=json&locations=%5B%7B%22geometry%22%3A%7B%22x%22%3A1,%22y%22%3A2%7D%7D,%7B%22geometry%22%3A%7B%22x%22%3A3,%22y%22%3A4%7D%7D%5D&outSR=26912&inSR=26912&tolerance=1");
    }
    [Fact]
    public void Should_create_valid_query_string_with_empty_locations() {
        // create new GeometryToMeasure.RequestContract with a RequestLocation
        var model = new GeometryToMeasure.RequestContract();

        // ?f=json&locations=[{"geometry":{"x":1,"y":2}},{"geometry":{"x":3,"y":4}}]&outSR=26912&inSR=26912&tolerance=1
        model.QueryString.ShouldBe("?f=json&locations=&outSR=26912&inSR=26912&tolerance=0");
    }
    [Fact]
    public void Should_create_valid_json_for_request_location() {
        var model = new GeometryToMeasure.RequestLocation(new Point(1, 2));

        model.ToString().ShouldBe("""{"geometry":{"x":1,"y":2}}""");
    }
    [Fact]
    public void Should_round_measure_location_to_4_places() {
        var model = new GeometryToMeasure.ResponseLocation { Measure = 1.23456789D };

        model.Measure.ShouldBe(1.2346);
    }
}
