using System.Text.Json.Serialization;
using AGRC.api.Models;
using Newtonsoft.Json;

namespace AGRC.api.Features.Milepost {
    public class RouteMilepostResponseContract {
        /// <summary>
        /// The datasource for the result
        /// </summary>
        /// <example>SGID10.TRANSPORTATION.UDOTROUTES_LRS</example>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        /// <summary>
        /// The geographic coordinates along the route for the given milepost measure
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public Point Location { get; set; }

        /// <summary>
        /// The route matched
        /// </summary>
        /// <example>Route 0015, Milepost 309.001</example>
        [JsonProperty(PropertyName = "matchRoute")]
        public string MatchRoute { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string InputRouteMilePost { get; set; }
    }
}
