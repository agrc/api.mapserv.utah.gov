using System;
using System.Net;
using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models.Configuration {
    public class ReverseLocatorConfiguration : UrlConfigurationBase {
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public bool ReverseGeocodes { get; set; }
        public string PathToLocator { get; set; } = "/arcgis/rest/services/Geolocators/";

        private const string Template = "{0}/GeocodeServer/reverseGeocode?f=json" +
                                        "&location={1},{2}&distance={3}&outSR={4}";

        public override string Url() {
            var host = base.GetHost();

            return $"{host}{PathToLocator}";
        }

        public LocatorProperties ToLocatorProperty(double x, double y, double distance, int wkid) {
            var url = Url();
            var geocodeUrl = url + string.Format(Template, ServiceName, x, y, distance, wkid);

            return new LocatorProperties {
                Url = geocodeUrl,
                Name = DisplayName
            };
        }
    }
}
