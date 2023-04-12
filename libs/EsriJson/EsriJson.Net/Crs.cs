using System.Text.Json.Serialization;

namespace EsriJson.Net;
public class Crs {
    [JsonPropertyName("wkid")]
    public int WellKnownId { get; set; }
}
