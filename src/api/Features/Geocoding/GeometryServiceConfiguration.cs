using ugrc.api.Models.Configuration;

namespace ugrc.api.Features.Geocoding;
public class GeometryServiceConfiguration : GisServerConfigurationBase {
    public string PathToGeometryServer { get; set; } = "/arcgis/rest/services/Utilities/Geometry/GeometryServer/";

    public override string Url() {
        var host = base.GetHost();

        return $"{host}{PathToGeometryServer}project/";
    }
}
