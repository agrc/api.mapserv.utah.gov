using EsriJson.Net;
using NetTopologySuite.Geometries;
using WebAPI.Common.Abstractions;
using System;
using System.Collections.Generic;
using ProjNet.CoordinateSystems.Transformations;

namespace WebAPI.API.Commands.Search
{
    public class NtsGeometryTransformerCommand : Command<EsriJsonObject>
    {
        public Geometry Geometry { get; }
        public ICoordinateTransformation Transformer { get; }
        public int WkId { get; } = 26912;

        public NtsGeometryTransformerCommand(Geometry geometry, int wkid)
        {
            Geometry = geometry;
            WkId = wkid;

            if (wkid != 26912)
            {
                var transformer = new CoordinateTransformationFactory();
                Transformer = transformer.CreateFromCoordinateSystems(ProjNet.SRID.SRIDReader.GetCSbyID(26912), ProjNet.SRID.SRIDReader.GetCSbyID(wkid));
            }
        }

        public override string ToString() => "NtsGeometryTransformerCommand";

        protected override void Execute()
        {
            if (Geometry is Point p1)
            {
                var point = new EsriJson.Net.Geometry.Point(p1.X, p1.Y);

                if (Transformer is not null)
                {
                    (var x, var y) = Transformer.MathTransform.Transform(point.X, point.Y);

                    point.X = x;
                    point.Y = y;
                }

                point.CRS = new Crs
                {
                    WellKnownId = WkId
                };

                Result = point;

                return;
            }

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
                    WellKnownId = WkId
                }
            };

            Result = poly;
        }

        private List<EsriJson.Net.Geometry.RingPoint[]> ExtractRings(Polygon polygon)
        {
            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();

            var exterior = polygon.ExteriorRing;
            ringPoints.Add(new EsriJson.Net.Geometry.RingPoint[exterior.Count]);

            for (var i = 0; i < exterior.Count; i++)
            {
                var coordinate = exterior.Coordinates[i];

                if (Transformer is not null)
                {
                    (var x, var y) = Transformer.MathTransform.Transform(coordinate.X, coordinate.Y);

                    coordinate.X = x;
                    coordinate.Y = y;
                }

                ringPoints[0][i] = new EsriJson.Net.Geometry.RingPoint(coordinate.X, coordinate.Y);
            }

            for (var i = 0; i < polygon.Holes.Length; i++)
            {
                var ring = polygon.Holes[i];
                ringPoints.Add(new EsriJson.Net.Geometry.RingPoint[ring.Count]);
                for (var j = 0; j < ring.Count; j++)
                {
                    var coordinate = ring.Coordinates[j];
                    if (Transformer is not null)
                    {
                        (var x, var y) = Transformer.MathTransform.Transform(coordinate.X, coordinate.Y);

                        coordinate.X = x;
                        coordinate.Y = y;
                    }

                    ringPoints[i + 1][j] = new EsriJson.Net.Geometry.RingPoint(coordinate.X, coordinate.Y);
                }
            }

            return ringPoints;
        }
    }
}