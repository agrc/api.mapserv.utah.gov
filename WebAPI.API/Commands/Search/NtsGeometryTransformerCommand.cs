using EsriJson.Net;
using NetTopologySuite.Geometries;
using WebAPI.Common.Abstractions;
using System;
using System.Collections.Generic;

namespace WebAPI.API.Commands.Search
{
    public class NtsGeometryTransformerCommand : Command<EsriJson.Net.Geometry.Polygon>
    {
        public Geometry Geometry { get; }

        public NtsGeometryTransformerCommand(Geometry geometry)
        {
            Geometry = geometry;
        }

        public override string ToString() => "NtsGeometryTransformerCommand";

        protected override void Execute()
        {
            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();
            if (Geometry is MultiPolygon multiPolygon)
            {
                foreach (var p in multiPolygon.Geometries)
                {
                    ringPoints.AddRange(ExtractRings(p as Polygon));
                }
            }
            else if (Geometry is Polygon polygon)
            {
                ringPoints = ExtractRings(polygon);
            }

            var poly = new EsriJson.Net.Geometry.Polygon(ringPoints)
            {
                CRS = new Crs
                {
                    WellKnownId = 26912
                }
            };

            Result = poly;
        }

        private static List<EsriJson.Net.Geometry.RingPoint[]> ExtractRings(Polygon polygon)
        {
            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();

            var exterior = polygon.ExteriorRing;
            ringPoints.Add(new EsriJson.Net.Geometry.RingPoint[exterior.Count]);

            for (var i = 0; i < exterior.Count; i++)
            {
                var coordinate = exterior.Coordinates[i];
                ringPoints[0][i] = new EsriJson.Net.Geometry.RingPoint(coordinate.X, coordinate.Y);
            }

            for (var i = 0; i < polygon.Holes.Length; i++)
            {
                var ring = polygon.Holes[i];
                ringPoints.Add(new EsriJson.Net.Geometry.RingPoint[ring.Count]);
                for (var j = 0; j < ring.Count; j++)
                {
                    var coordinate = ring.Coordinates[j];
                    ringPoints[i + 1][j] = new EsriJson.Net.Geometry.RingPoint(coordinate.X, coordinate.Y);
                }
            }

            return ringPoints;
        }
    }
}