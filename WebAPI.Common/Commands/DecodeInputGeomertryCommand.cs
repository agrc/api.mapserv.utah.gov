using System.Text.Json;
using WebAPI.Common.Abstractions;

namespace WebAPI.Common.Commands
{
    public class DecodeInputGeomertryCommand : Command<string>
    {
        private readonly string Geometry;
        public DecodeInputGeomertryCommand(string geometry)
        {
            Geometry = geometry;
        }

        protected override void Execute()
        {
            if (string.IsNullOrEmpty(Geometry))
            {
                return;
            }
            var geometry = Geometry.ToUpper().Replace(" ", "").Trim();

            if (geometry[0] == 'P')
            {
                // have a point (5) polyline (8) or polygon (7)
                var colon = Geometry.IndexOf(':');
                if (colon < 5)
                {
                    // error;
                }

                if (colon == 5)
                {
                    // type == point
                    if (Geometry[colon + 1] == '[')
                    {
                        // legacy point:[x,y]
                        var start = colon + 2;
                        var distance = geometry.Length - start - 1;

                        geometry = geometry.Substring(start, distance);
                        geometry = geometry.Replace(',', ' ');
                    }
                    else if (geometry[colon + 1] == '{')
                    {
                        // esri geom point:{"x" : <x>, "y" : <y>, "z" : <z>, "m" : <m>, "spatialReference" : {<spatialReference>}}
                        var point = JsonSerializer.Deserialize<Domain.ArcServerResponse.Geolocator.Location>(geometry.Substring(colon + 1, geometry.Length - colon - 1));
                        geometry = $"{point.X} {point.Y}";
                    }
                }
                else if (colon == 7)
                {
                    // type == polygon
                }
                else
                {
                    // type == polyline
                }
            }

            Result = geometry;
        }

        public override string ToString()
        {
            return "DecodeInputGeometry";
        }
    }
}
