using AGRC.api.Models.ArcGis;

namespace api.tests.Models.ArcGis;

public class MeasureToGeometryTests {
    [Fact]
    public void Should_create_valid_query_string() {
        // create new MeasureToGeometry.RequestContract with a RequestLocation
        var model = new MeasureToGeometry.RequestContract {
            Locations = [
                new MeasureToGeometry.RequestLocation("300", "0015PM"),
            ],
            OutSr = 102100
        };

        // ?f=json&locations=[{"routeId":"0015PM","measure":"300"}]&outSR=102100
        model.QueryString.ShouldBe("?f=json&locations=%5B%7B%22routeId%22%3A%220015PM%22,%22measure%22%3A%22300%22%7D%5D&outSR=102100");
    }
    [Fact]
    public void Should_create_valid_query_string_with_multiple_locations() {
        // create new MeasureToGeometry.RequestContract with a RequestLocation
        var model = new MeasureToGeometry.RequestContract {
            Locations = [
                new MeasureToGeometry.RequestLocation("300", "0015PM"),
                new MeasureToGeometry.RequestLocation("1", "0015PM"),
            ],
            OutSr = 102100
        };

        // ?f=json&locations=[{"measure":"300","routeId":"0015PM"},{"measure":"1","routeId":"0015PM"}]&outSR=102100
        model.QueryString.ShouldBe("?f=json&locations=%5B%7B%22routeId%22%3A%220015PM%22,%22measure%22%3A%22300%22%7D,%7B%22routeId%22%3A%220015PM%22,%22measure%22%3A%221%22%7D%5D&outSR=102100");
    }
    [Fact]
    public void Should_create_valid_query_string_with_empty_locations() {
        // create new MeasureToGeometry.RequestContract with a RequestLocation
        var model = new MeasureToGeometry.RequestContract {
            OutSr = 102100
        };

        // ?f=json&locations=[{"routeId":"0015PM","measure":"300"},{"routeId":"0015PM","measure":"100"}]&outSR=102100
        model.QueryString.ShouldBe("?f=json&locations=&outSR=102100");
    }
    [Fact]
    public void Should_create_valid_json_for_request_location() {
        var model = new MeasureToGeometry.RequestLocation("300", "0015PM");

        model.ToString().ShouldBe("""{"routeId":"0015PM","measure":"300"}""");
    }
    [Fact]
    public void Should_strip_leading_zeros_and_trailing_m() {
        var model = new MeasureToGeometry.ResponseLocation(MeasureToGeometry.Status.esriLocatingOK, Array.Empty<MeasureToGeometry.ResponseLocation>(), "0001PM", GeometryType.esriGeometryPoint, null);

        model.RouteId.ShouldBe("1P");
    }
}
