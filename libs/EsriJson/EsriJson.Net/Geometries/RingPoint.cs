using System.Text.Json.Serialization;
using EsriJson.Net.Geometry.Converters;

namespace EsriJson.Net.Geometry;
[JsonConverter(typeof(RingPointConverter))]
public class RingPoint(double x, double y) {
    public double X { get; set; } = x;

    public double Y { get; set; } = y;

    public bool Equals(RingPoint obj) => obj != null && obj.X == X && obj.Y == Y;
}
