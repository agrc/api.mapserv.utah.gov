using AGRC.api.Models;
using Newtonsoft.Json;

namespace AGRC.api.Features.Milepost {
    public class RouteMilepostResponseContract {
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "location")]
        public Point Location { get; set; }

        [JsonProperty(PropertyName = "matchRoute")]
        public string MatchRoute { get; set; }

        [JsonIgnore]
        public string InputRouteMilePost { get; set; }
    }
}
