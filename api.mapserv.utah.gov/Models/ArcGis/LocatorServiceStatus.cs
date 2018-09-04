namespace api.mapserv.utah.gov.Models.ArcGis {
    public class LocatorServiceStatus : RestErrorable {
        public double CurrentVersion { get; set; }

        public string Capabilities { get; set; }
    }
}
