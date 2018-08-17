using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models.ArcGis {
    public class LocatorServiceStatus {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "details")]
        public string[] Details { get; set; }
    }
}
