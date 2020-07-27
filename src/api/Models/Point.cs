using Newtonsoft.Json;

namespace AGRC.api.Models {
    /// <summary>
    ///     Point model for points
    /// </summary>
    public class Point {
        public Point() {
        }

        public Point(double x, double y) {
            X = x;
            Y = y;
        }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        public string ToQuery() =>
            $"geometries={{\"geometryType\" : \"esriGeometryPoint\",\"geometries\" : [{{\"x\" :{X}, \"y\" : {Y}}}]}}";
    }
}
