using AGRC.api.Infrastructure;
using EsriJson.Net;
using NetTopologySuite.Geometries;
using static AGRC.api.Features.Converting.EsriGraphic;

namespace AGRC.api.Features.Searching;
public class NtsToEsriMapper {
    public class Computation : IComputation<SerializableGraphic> {
        internal readonly Geometry Geometry;

        public Computation(Geometry geometry) {
            Geometry = geometry;
        }
    }

    public class Handler : IComputationHandler<Computation, SerializableGraphic> {
        public Task<SerializableGraphic> Handle(Computation computation, CancellationToken _) {
            var ringPoints = new List<EsriJson.Net.Geometry.RingPoint[]>();
            if (computation.Geometry is MultiPolygon multiPolygon) {
                foreach (var p in multiPolygon.Geometries) {
                    ringPoints.AddRange(ExtractRings(p as Polygon));
                }
            } else if (computation.Geometry is Polygon polygon) {
                ringPoints = ExtractRings(polygon);
            }

            var poly = new EsriJson.Net.Geometry.Polygon(ringPoints) {
                CRS = new Crs {
                    WellKnownId = 26912
                }
            };

            var serializableGraphic = new SerializableGraphic(new Graphic(poly, new Dictionary<string, object>()));

            return Task.FromResult(serializableGraphic);
        }

        private static List<EsriJson.Net.Geometry.RingPoint[]> ExtractRings(Polygon polygon) {
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
    }
}
