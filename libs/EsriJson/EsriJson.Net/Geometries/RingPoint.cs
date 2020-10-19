using System.Text.Json.Serialization;
using EsriJson.Net.Geometry.Converters;

namespace EsriJson.Net.Geometry {
    [JsonConverter(typeof(RingPointConverter))]
    public class RingPoint {
        public double X { get; set; }

        public double Y { get; set; }

        public RingPoint(double x, double y) {
            X = x;
            Y = y;
        }

        public bool Equals(RingPoint obj) => obj != null && obj.X == X && obj.Y == Y;
    }
}
