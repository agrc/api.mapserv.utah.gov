using Newtonsoft.Json;

namespace WebAPI.Domain.InputOptions
{
    public class AgoGeocodeOptions
    {
        [JsonProperty(PropertyName = "Single Line Input")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "maxLocations")]
        public int SuggestCount { get; set; }

        public string WkId { get; set; }

        public string F { get; set; }
    }
}