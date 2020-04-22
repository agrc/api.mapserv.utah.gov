using System;
using System.Net;
using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models.Configuration {
    public class LocatorConfiguration : GisServerConfigurationBase {
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public bool ReverseGeocodes { get; set; }
        public LocatorType LocatorType { get; set; }
        public int Weight { get; set; } = 0;
        public string PathToLocator { get; set; } = "/arcgis/rest/services/Geolocators/";

        private const string Template = "{0}/GeocodeServer/findAddressCandidates?f=json" +
                                        "&Street={1}" +
                                        "&City={2}" +
                                        "&outSR={3}";
        public override string Url() {
            var host = base.GetHost();

            return $"{host}{PathToLocator}";
        }

        public LocatorProperties ToLocatorProperty(GeocodeInput address, Func<GeocodeInput, string> addressResolver) => ToLocatorPropertyBase(Template, address.Weight, ServiceName, WebUtility.UrlEncode(addressResolver(address)), address.Grid, address.WkId);

        private LocatorProperties ToLocatorPropertyBase(string template, int weight, params object[] args) {
            var url = Url();
            var geocodeUrl = url + string.Format(template, args);

            return new LocatorProperties {
                Url = geocodeUrl,
                Name = DisplayName
            };
        }
    }
}
