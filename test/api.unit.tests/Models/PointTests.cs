using AGRC.api.Models;

namespace api.tests.Models;
public class PointTests {
    [Fact]
    public void Should_create_esri_geometry_json() {
        const string json = """{"x":1,"y":0,"spatialReference":{"wkid":26912}}""";

        var point = new PointWithSpatialReference(1, 0, new(26912, null));

        point.ToEsriJson().ShouldBe(json);
    }
    [Fact]
    public void Should_create_pg_geometry() {
        const string json = """st_pointfromtext('POINT(1 0)',26912)""";

        var point = new PointWithSpatialReference(1, 0, new(26912, null));

        point.ToPostGis().ShouldBe(json);
    }
    [Fact]
    public void Should_create_pg_geometry_with_other_srid() {
        const string json = """st_transform(st_pointfromtext('POINT(1 0)',3857),26912)""";

        var point = new PointWithSpatialReference(1, 0, new(3857, null));

        point.ToPostGis().ShouldBe(json);
    }
    [Fact]
    public void Should_create_simple_coordinate_pair() {
        var point = new PointWithSpatialReference(1, 0, new(3857, null));

        point.ToString().ShouldBe("1,0");
    }
}
