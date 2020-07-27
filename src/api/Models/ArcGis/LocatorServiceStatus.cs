namespace AGRC.api.Models.ArcGis {
    public class LocatorServiceStatus : RestErrorable {
        public double CurrentVersion { get; set; }

        public string Capabilities { get; set; }
    }
}
