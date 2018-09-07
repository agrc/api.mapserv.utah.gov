using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.mapserv.utah.gov.Models.ApiResponses {
    public class SearchApiResponse {
        [JsonProperty(PropertyName = "geometry")]
        public JObject Geometry { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public IDictionary<string, object> Attributes { get; set; }
    }
}
