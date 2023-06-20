using AGRC.api.Models;
using AGRC.api.Models.Configuration;

namespace AGRC.api.Features.Geocoding;
public class ReverseLocatorConfiguration : GisServerConfigurationBase {
    public string DisplayName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public bool ReverseGeocodes { get; set; }
    public string PathToLocator { get; set; } = "/arcgis/rest/services/Geolocators/";

    private const string Template = "{0}/GeocodeServer/reverseGeocode?f=json" +
                                    "&location={1}&distance={2}&outSR={3}";

    public override string Url() {
        var host = base.GetHost();

        return $"{host}{PathToLocator}";
    }

    public LocatorProperties ToLocatorProperty(PointWithSpatialReference point, double distance, int wkid) {
        var url = Url();
        var geocodeUrl = url + string.Format(Template, ServiceName, point.ToEsriJson(), distance, wkid);

        return new LocatorProperties(geocodeUrl, DisplayName);
    }
}
