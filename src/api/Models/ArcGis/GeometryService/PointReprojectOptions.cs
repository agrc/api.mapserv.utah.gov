using System.Text.Json.Serialization;

namespace AGRC.api.Models.ArcGis;
public class PointReprojectOptions(int currentSpatialReference, int reprojectToSpatialReference,
                             IReadOnlyCollection<double> coordinates) {
    [JsonPropertyName("inSR")]
    public int CurrentSpatialReference { get; } = currentSpatialReference;

    [JsonPropertyName("outSR")]
    public int ReprojectToSpatialReference { get; } = reprojectToSpatialReference;

    [JsonPropertyName("geometries")]
    public IReadOnlyCollection<double> Coordinates { get; } = coordinates;
}
