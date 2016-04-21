using Newtonsoft.Json;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.Domain.ApiResponses
{
    public class RouteMilepostResult
    {
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        [JsonProperty(PropertyName = "matchRoute")]
        public string MatchRoute { get; set; }

        [JsonIgnore]
        public string InputRouteMilePost { get; set; }
    }
}