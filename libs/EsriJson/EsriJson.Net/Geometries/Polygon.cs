using System;
using System.Collections.Generic;

namespace EsriJson.Net.Geometry {
    public class Polygon : EsriJsonObject {
        public Polygon(List<RingPoint[]> rings) {
            ValidateRings(rings);

            Rings = rings;
        }

        public Polygon() {
            Rings = new List<RingPoint[]>();
        }

        public List<RingPoint[]> Rings { get; private set; }

        private static void ValidateRings(IEnumerable<RingPoint[]> rings) {
            foreach (var ringPoints in rings) {
                var length = ringPoints.Length;

                if (length < 3)
                    throw new ArgumentException("Rings are made up of three or more points. Yours has less.");

                var startpoint = ringPoints[0];
                var endpoint = ringPoints[length - 1];

                if (!startpoint.Equals(endpoint)) {
                    throw new ArgumentException(
                                                "A ring must be explicitly closed. The first and last point must be the same.");
                }
            }
        }

        public void AddRing(IList<RingPoint[]> rings) {
            ValidateRings(rings);

            foreach (var ring in rings) {
                Rings.Add(ring);
            }
        }

        public override string Type => "polygon";
    }
}
