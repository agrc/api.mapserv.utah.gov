using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using EsriJson.Net.CoordinateReferenceSystem;
using EsriJson.Net.Geometry;
using Newtonsoft.Json;
using Soe.Common.Infastructure.Commands;
using IGeometry = ESRI.ArcGIS.Geometry.IGeometry;
using Point = EsriJson.Net.Geometry.Point;
using Polygon = EsriJson.Net.Geometry.Polygon;
using Polyline = EsriJson.Net.Geometry.Polyline;

namespace WebAPI.Search.Soe.Commands
{
    public class CreateGraphicFromGeometryCommand : Command<string>
    {
        public CreateGraphicFromGeometryCommand(IGeometry geometry, ISpatialReference spatialReference)
        {
            Geometry = geometry;
            SpatialReference = spatialReference;
        }

        public IGeometry Geometry { get; set; }
        public ISpatialReference SpatialReference { get; set; }

        public override string ToString()
        {
            return "CreateGraphicFromGeometry";
        }

        protected override void Execute()
        {
            if (Geometry is IEnvelope)
            {
                var polygon = new Polygon
                    {
                        CRS = new CRS
                            {
                                WellKnownId = 26912
                            }
                    };

                ISegmentCollection segmentColl = new PolygonClass();

                segmentColl.SetRectangle(Geometry as IEnvelope);
                var poly = segmentColl as IPolygon4;

                if (SpatialReference != null)
                {
                    poly.Project(SpatialReference);
                    polygon.CRS.WellKnownId = SpatialReference.FactoryCode;
                }

                var geometryCollection = poly as IGeometryCollection;
                var count = geometryCollection.GeometryCount;

                for (var i = 0; i < count; i++)
                {
                    var points = geometryCollection.Geometry[i] as IPointCollection4;

                    var pointCount = points.PointCount;
                    var ringPoints = new List<RingPoint>(pointCount);

                    for (var j = 0; j < pointCount; j++)
                    {
                        var point = points.Point[j];
                        ringPoints.Add(new RingPoint(point.X, point.Y));
                    }

                    polygon.AddRing(new List<RingPoint[]>
                        {
                            ringPoints.ToArray()
                        });
                }

                Result = JsonConvert.SerializeObject(polygon);
                return;
            }

            if (Geometry is IPoint)
            {
                var esriPoint = Geometry as IPoint;
                var point = new Point
                    {
                        CRS = new CRS
                            {
                                WellKnownId = 26912
                            }
                    };

                if (SpatialReference != null)
                {
                    esriPoint.Project(SpatialReference);
                    point.CRS.WellKnownId = SpatialReference.FactoryCode;
                }

                point.X = esriPoint.X;
                point.Y = esriPoint.Y;

                Result = JsonConvert.SerializeObject(point);
                return;
            }

            if (Geometry is IPolyline)
            {
                var line = new Polyline
                {
                    CRS = new CRS
                    {
                        WellKnownId = 26912
                    }
                };

                var simpleGeom = Geometry as IPolyline5;
                simpleGeom.Generalize(10);

                if (SpatialReference != null)
                {
                    simpleGeom.Project(SpatialReference);
                    line.CRS.WellKnownId = SpatialReference.FactoryCode;
                }

                var geometryCollection = simpleGeom as IGeometryCollection;
                var count = geometryCollection.GeometryCount;

                for (var i = 0; i < count; i++)
                {
                    var points = geometryCollection.Geometry[i] as IPointCollection4;

                    var pointCount = points.PointCount;
                    var ringPoints = new List<RingPoint>(pointCount);

                    for (var j = 0; j < pointCount; j++)
                    {
                        var point = points.Point[j];
                        ringPoints.Add(new RingPoint(point.X, point.Y));
                    }

                    line.AddPath(new List<RingPoint[]>
                        {
                            ringPoints.ToArray()
                        });
                }

                Result = JsonConvert.SerializeObject(line);
                return;
            }

            if (Geometry is IPolygon)
            {
                var polygon = new Polygon
                {
                    CRS = new CRS
                    {
                        WellKnownId = 26912
                    }
                };

                var simpleGeom = Geometry as IPolygon4;
                simpleGeom.Generalize(10);

                if (SpatialReference != null)
                {
                    simpleGeom.Project(SpatialReference);
                    polygon.CRS.WellKnownId = SpatialReference.FactoryCode;
                }

                var geometryCollection = simpleGeom as IGeometryCollection;
                var count = geometryCollection.GeometryCount;

                for (var i = 0; i < count; i++)
                {
                    var points = geometryCollection.Geometry[i] as IPointCollection4;

                    var pointCount = points.PointCount;
                    var ringPoints = new List<RingPoint>(pointCount);

                    for (var j = 0; j < pointCount; j++)
                    {
                        var point = points.Point[j];
                        ringPoints.Add(new RingPoint(point.X, point.Y));
                    }

                    polygon.AddRing(new List<RingPoint[]>
                        {
                            ringPoints.ToArray()
                        });
                }

                Result = JsonConvert.SerializeObject(polygon);
            }
        }
    }
}