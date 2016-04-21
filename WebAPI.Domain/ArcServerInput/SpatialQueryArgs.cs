using Newtonsoft.Json;
using WebAPI.Domain.InputOptions;

namespace WebAPI.Domain.ArcServerInput
{
    public class SpatialQueryArgs
    {
        [JsonProperty(PropertyName = "featureClass")]
        public string FeatureClass { get; set; }

        [JsonProperty(PropertyName = "returnValues")]
        public string ReturnValues { get; set; }

        [JsonProperty(PropertyName = "geometry")]
        public string Geometry { get; set; }

        [JsonProperty(PropertyName = "predicate")]
        public string Predicate { get; set; }

        [JsonProperty(PropertyName = "wkid")]
        public int WkId { get; set; }

        [JsonProperty(PropertyName = "buffer")]
        public double Buffer { get; set; }

        public SpatialQueryArgs(string featureClass, string returnValues, SearchOptions options)
        {
            FeatureClass = featureClass;
            ReturnValues = returnValues;
            Geometry = options.Geometry;
            Predicate = options.Predicate;
            WkId = options.WkId;
            Buffer = options.Buffer;
        }
    }
}