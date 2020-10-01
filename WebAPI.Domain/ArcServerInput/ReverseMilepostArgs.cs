using Newtonsoft.Json;
using WebAPI.Domain.InputOptions;

namespace WebAPI.Domain.ArcServerInput
{
    public class ReverseMilepostArgs
    {
        public ReverseMilepostArgs(double x, double y, ReverseMilepostOptions options)
        {
            X = x;
            Y = y;
            WkId = options.WkId;
            Buffer = options.Buffer;
            IncludeRamps = options.IncludeRampSystems;
        }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        [JsonProperty(PropertyName = "wkid")]
        public int WkId { get; set; }

        [JsonProperty(PropertyName = "buffer")]
        public double Buffer { get; set; }

        [JsonProperty(PropertyName = "includeRamps")]
        public int IncludeRamps { get; set; }
    }
}