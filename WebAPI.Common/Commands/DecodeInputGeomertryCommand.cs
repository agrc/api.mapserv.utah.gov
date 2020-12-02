using System;
using System.Text.Json;
using WebAPI.Common.Abstractions;

namespace WebAPI.Common.Commands
{
    public class JsApiPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public JsApiPointSpatialReference SpatialReference { get; set; }

        public string ToSql => $"{X} {Y}";
    }

    public class JsApiPointSpatialReference
    {
        private int wkid;

        public int Wkid { get { 
                if (LatestWkid > 0)
                {
                    return LatestWkid;
                }

                return wkid;
            } 
            set => wkid = value; }
        public int LatestWkid { get; set; }
    }
    public class DecodeInputGeomertryCommand : Command<JsApiPoint>
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
                        var coordinates = geometry.Split(' ');

                        try
                        {
                            Result = new JsApiPoint
                            {
                                X = Convert.ToDouble(coordinates[0]),
                                Y = Convert.ToDouble(coordinates[1])
                            };
                        }
                        catch (FormatException)
                        {
                            ErrorMessage = "GEOMETRY COORDINATES APPEAR TO BE INVALID.";

                            if (coordinates[0].ToLower() == "nan" ||
                                coordinates[1].ToLower() == "nan")
                            {
                                ErrorMessage = "THE OPERATION WAS ATTEMPTED ON AN EMPTY GEOMETRY.";
                            }
                        }
                    }
                    else if (geometry[colon + 1] == '{')
                    {
                        // esri geom point:{"x" : <x>, "y" : <y>, "z" : <z>, "m" : <m>, "spatialReference" : {<spatialReference>}}
                        Result = JsonSerializer.Deserialize<JsApiPoint>(geometry.Substring(colon + 1, geometry.Length - colon - 1), new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        });
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
        }

        public override string ToString()
        {
            return "DecodeInputGeometry";
        }
    }
}
