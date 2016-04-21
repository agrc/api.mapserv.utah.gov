using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    /// <summary>
    ///     Location model for points
    /// </summary>
    public class Location
    {
        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        public Location()
        {
            
        }

        public Location(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// To the query.
        /// </summary>
        /// geometries=%7B%0D%0A%22geometryType%22+%3A+%22esriGeometryPoint%22%2C%0D%0A%22geometries%22+%3A+%5B%0D%0A+%7B%22x%22+%3A+425058.902953965%2C+%22y%22+%3A+4508803.55788463%7D%0D%0A%5D%0D%0A%7D&transformation=&transformForward=false&f=html
        /// <returns></returns>
        public string ToQuery()
        {
            return string.Format("geometries={{\"geometryType\" : \"esriGeometryPoint\",\"geometries\" : [{{\"x\" :{0}, \"y\" : {1}}}]}}", X, Y);
        }
    }
}