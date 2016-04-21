using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebAPI.Domain.ApiResponses
{
    public class SearchResult
    {
        [JsonProperty(PropertyName = "geometry")]
        public JObject Geometry { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<string, object> Attributes { get; set; }
    }
}