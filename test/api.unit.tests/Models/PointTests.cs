using AGRC.api.Models;

namespace api.tests.Models;
public class PointTests {
    [Fact]
    public void Should_create_esri_rest_geometry_json() {
        const string json = """geometries={"geometryType":"esriGeometryPoint","geometries":[{"x":1,"y":0}]}""";

        var point = new Point(1, 0);

        point.ToQuery().ShouldBe(json);
    }
}
