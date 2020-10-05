using WebAPI.Common.Models.Esri.Errors;

namespace WebAPI.Common.Models.Esri.RoadsAndHighways
{
    public class Concurrencies
    {
        public class RequestContract
        {
            public RequestLocation[] Locations { get; set; }

            public override string ToString()
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

        public class LocationBase
        {
            public string RouteId { get; set; }
            public double FromMeasure { get; set; }
            public double ToMeasure { get; set; }

            public override string ToString()
                => $"{{\"routeId\":\"{RouteId}\",\"fromMeasure\":\"{FromMeasure}\",\"toMeasure\":\"{ToMeasure}\"}}";
        }

        public class RequestLocation : LocationBase { }

        public class ResponseContract : Errorable
        {
            public ResponseLocations[] Locations { get; set; }
        }

        public class ResponseLocations : LocationBase
        {
            public ConcurrencyLocations[] Concurrencies { get; set; }
        }

        public class ConcurrencyLocations : LocationBase
        {
            public bool IsDominant { get; set; }
        }
    }
}
