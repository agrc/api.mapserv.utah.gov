using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGRC.api.Features.Searching {
    public class SearchResponseContract {
        [JsonProperty(PropertyName = "geometry")]
        public JObject Geometry { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public IDictionary<string, object> Attributes { get; set; }
    }
}
