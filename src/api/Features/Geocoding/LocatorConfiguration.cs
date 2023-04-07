using System.Net;
using AGRC.api.Models.Configuration;
using AGRC.api.Models.Constants;

namespace AGRC.api.Features.Geocoding;
public class LocatorConfiguration : GisServerConfigurationBase {
    public string DisplayName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public bool ReverseGeocodes { get; set; }
    public LocatorType LocatorType { get; set; }
    public int Weight { get; set; }
    public string PathToLocator { get; set; } = "/arcgis/rest/services/Geolocators/";

    private const string Template = "{0}/GeocodeServer/findAddressCandidates?f=json" +
                                    "&matchOutOfRange=false" +
                                    "&outFields=addr_type,addnum" +
                                    "&outSR={3}" +
                                    "&Address={1}" +
                                    "&City={2}";
    public override string Url() {
        var host = base.GetHost();

        return $"{host}{PathToLocator}";
    }

    public LocatorProperties ToLocatorProperty(LocatorMetadata address, Func<LocatorMetadata, string> addressResolver) =>
        ToLocatorPropertyBase(Template, address.Weight, ServiceName, WebUtility.UrlEncode(addressResolver(address)), address.Grid, address.WkId);

    private LocatorProperties ToLocatorPropertyBase(string template, int weight, params object[] args) {
        var url = Url();
        var geocodeUrl = url + string.Format(template, args);

        return new LocatorProperties(geocodeUrl, DisplayName, weight);
    }
}
