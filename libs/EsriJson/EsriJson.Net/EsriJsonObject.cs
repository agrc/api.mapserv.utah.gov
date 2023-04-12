using System.Text.Json.Serialization;

namespace EsriJson.Net {
    public abstract class EsriJsonObject {
        [JsonPropertyName("spatialReference")]
        public Crs CRS { get; set; }

        public abstract string Type { get; }
    }
}
