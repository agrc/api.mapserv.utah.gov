using ugrc.api.Models;
using ugrc.api.Models.ArcGis;

namespace api.tests.Models.ArcGis;
public class ImageServiceIdentifyTests {
    [Fact]
    public void Should_build_esri_rest_parameters() {
        var contract = new ImageServiceIdentify.RequestContract(new PointWithSpatialReference(1, 2, new(3, null)), GeometryType.esriGeometryPoint);
        contract.ToQuery().ShouldBe("""?geometry={"x":1,"y":2,"spatialReference":{"wkid":3}}&geometryType=esriGeometryPoint&returnGeometry=false&returnCatalogItems=false&f=json""");
    }
    [Fact]
    public void Should_calculate_feet_from_meters() {
        var contract = new ImageServiceIdentify.ResponseContract("100", null);
        contract.Feet.ShouldBe("328.084");
    }
    [Fact]
    public void Should_return_no_data_for_non_numeric() {
        var contract = new ImageServiceIdentify.ResponseContract("a", null);
        contract.Feet.ShouldBe("NoData");
    }
}
