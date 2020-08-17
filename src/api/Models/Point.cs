using Newtonsoft.Json;

namespace AGRC.api.Models {
    /// <summary>
    /// A coordinate pair collection representing a point
    /// </summary>
    public class Point {
        public Point() {
        }

        public Point(double x, double y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// A geographic coordinate representing the longitude or easting
        /// </summary>
        /// <example>425961.8481116538</example>
        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        /// <summary>
        /// A geographic coordinate representing the latitude or northing
        /// </summary>
        /// <example>4514356.332043296</example>
        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        public string ToQuery() =>
            $"geometries={{\"geometryType\" : \"esriGeometryPoint\",\"geometries\" : [{{\"x\" :{X}, \"y\" : {Y}}}]}}";
    }
}
