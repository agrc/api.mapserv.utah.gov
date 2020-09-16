using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebAPI.Common.Models.Esri.Errors;

namespace WebAPI.Common.Models.Esri.RoadsAndHighways
{
    public class GeometryToMeasure
    {
        public class RequestContract
        {
            /// <summary>
            ///  [{"geometry":{"x":423622,"y":4509387}}]
            /// </summary>
            /// <value></value>
            public RequestLocation[] Locations { get; set; }
            public double Tolerance { get; set; }
            public int OutSr { get; set; } = 26912;
            public int InSr { get; set; } = 26912;

            public string QueryString
            {
                get
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query.Add("f", "json");
                    query.Add("locations", LocationsAsQuery());
                    query.Add("outSR", OutSr.ToString());
                    query.Add("inSR", InSr.ToString());
                    query.Add("tolerance", Tolerance.ToString());

                    return "?" + query.ToString();
                }
            }

            internal string LocationsAsQuery()
            {
                var locations = new string[Locations.Length];
                for (var i = 0; i < Locations.Length; i++)
                {
                    var location = Locations[i];
                    locations[i] = location.ToString();
                }

                return $"[{string.Join(",", locations)}]";
            }
        }

        public class RequestLocation
        {
            public Point Geometry { get; set; }

            public override string ToString()
                => $"{{\"geometry\":{{\"x\":{Geometry.X},\"y\":{Geometry.Y}}}}}";
        }

        public enum Status
        {
            // Locating was successful. {
            esriLocatingOK,
            // Locating was successful, and the input point was located on more than one route.
            esriLocatingMultipleLocation,
            // The route does not exist.
            esriLocatingCannotFindRoute,
            // The route does not have a shape or the shape is empty.
            esriLocatingRouteShapeEmpty,
            // The route does not have measures or the measures are null.
            esriLocatingRouteMeasuresNull,
            // The route is not an m-aware polyline.
            esriLocatingRouteNotMAware,
            // Could not find the route location's shape (the route has no measures or the route location's measures do not exist on the route).
            esriLocatingCannotFindLocation,
        }

        public class ResponseContract : Errorable
        {
            public ResponseLocation[] Locations { get; set; }
        }

        public class ResponseLocation
        {
            public ResponseLocation[] Results { get; set; }
            public Status Status { get; set; }
            public string RouteId { get; set; }
            public double Measure { get; set; }
            public Point Geometry { get; set; }
        }

        public class Point
        {
            public Point()
            {
            }

            public Point(double x, double y)
            {
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
}
