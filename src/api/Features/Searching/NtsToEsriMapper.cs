using AGRC.api.Infrastructure;
using EsriJson.Net;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Searching;
public class NtsToEsriMapper {
    public class Computation : IComputation<object?> {
        internal readonly Geometry Geometry;

        public Computation(Geometry geometry) {
            Geometry = geometry;
        }
    }

    public class Handler : IComputationHandler<Computation, object?> {
        private readonly ILogger? _log;
        public Handler(ILogger log) {
            _log = log?.ForContext<NtsToEsriMapper>();
        }
        public Task<object?> Handle(Computation computation, CancellationToken _) {
            if (computation.Geometry.SRID == 0) {
                computation.Geometry.SRID = 26912;
            }

            object? result = computation.Geometry switch {
                Point => new EsriJson.Net.Geometry.Point(computation.Geometry.Coordinate.X, computation.Geometry.Coordinate.Y) {
                    CRS = new Crs {
                        WellKnownId = computation.Geometry.SRID
                    }
                },
                MultiPoint => new EsriJson.Net.Geometry.MultiPoint(computation.Geometry.Coordinates.Select(x => new EsriJson.Net.Geometry.Point(x.X, x.Y)).ToArray()) {
                    CRS = new Crs {
                        WellKnownId = computation.Geometry.SRID
                    }
                },
                LineString => new EsriJson.Net.Geometry.Polyline(new List<EsriJson.Net.Geometry.RingPoint[]> { ExtractLineRingPoints(computation.Geometry as LineString) }) {
                    CRS = new Crs {
                        WellKnownId = computation.Geometry.SRID
                    }
                },
                MultiLineString => CreateLine(computation.Geometry as MultiLineString),
                Polygon => new EsriJson.Net.Geometry.Polygon(ExtractRings(computation.Geometry as Polygon)) {
                    CRS = new Crs {
                        WellKnownId = computation.Geometry.SRID
                    }
                },
                MultiPolygon => CreatePolygon(computation.Geometry as MultiPolygon),
                _ => null,
            };

            if (result is null) {
                _log?.ForContext("type", computation.Geometry.GeometryType)
                    .ForContext("ogcType", computation.Geometry.OgcGeometryType)
                    .Warning("requires action::could not convert geometry type");
            }

            return Task.FromResult(result);
        }

        private static EsriJson.Net.Geometry.RingPoint[] ExtractLineRingPoints(Geometry? lineString) =>
            lineString?.Coordinates.Select(x => new EsriJson.Net.Geometry.RingPoint(x.X, x.Y)).ToArray() ?? Array.Empty<EsriJson.Net.Geometry.RingPoint>();
        private static EsriJson.Net.Geometry.Polyline CreateLine(MultiLineString? multiLineString) {
            if (multiLineString is null) {
                return new EsriJson.Net.Geometry.Polyline();
            }

            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();
            foreach (var lineString in multiLineString.Geometries) {
                ringPoints.Add(ExtractLineRingPoints(lineString));
            }

            return new EsriJson.Net.Geometry.Polyline(ringPoints) {
                CRS = new Crs {
                    WellKnownId = multiLineString.SRID
                }
            };
        }
        private static List<EsriJson.Net.Geometry.RingPoint[]> ExtractRings(Polygon? polygon) {
            if (polygon is null) {
                return new List<EsriJson.Net.Geometry.RingPoint[]>();
            }

            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();

            var exterior = polygon.ExteriorRing;
            ringPoints.Add(new EsriJson.Net.Geometry.RingPoint[exterior.Count]);

            for (var i = 0; i < exterior.Count; i++) {
                var coordinate = exterior.Coordinates[i];
                ringPoints[0][i] = new EsriJson.Net.Geometry.RingPoint(coordinate.X, coordinate.Y);
            }

            for (var i = 0; i < polygon.Holes.Length; i++) {
                var ring = polygon.Holes[i];
                ringPoints.Add(new EsriJson.Net.Geometry.RingPoint[ring.Count]);
                for (var j = 0; j < ring.Count; j++) {
                    var coordinate = ring.Coordinates[j];
                    ringPoints[i + 1][j] = new EsriJson.Net.Geometry.RingPoint(coordinate.X, coordinate.Y);
                }
            }

            return ringPoints;
        }
        private static EsriJson.Net.Geometry.Polygon CreatePolygon(MultiPolygon? multiPolygon) {
            if (multiPolygon is null) {
                return new EsriJson.Net.Geometry.Polygon();
            }

            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();
            foreach (var polygon in multiPolygon.Geometries) {
                ringPoints.AddRange(ExtractRings(polygon as Polygon));
            }

            return new EsriJson.Net.Geometry.Polygon(ringPoints) {
                CRS = new Crs {
                    WellKnownId = multiPolygon.SRID
                }
            };
        }
    }
}
