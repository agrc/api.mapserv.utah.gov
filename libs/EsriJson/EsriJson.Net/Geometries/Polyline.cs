using System;
using System.Collections.Generic;
using System.Linq;

namespace EsriJson.Net.Geometry {
    public class Polyline : EsriJsonObject {
        public List<RingPoint[]> Paths { get; set; }

        public Polyline(List<RingPoint[]> paths) {
            ValidatePaths(paths);

            Paths = paths;
        }

        public Polyline() {
            Paths = new List<RingPoint[]>();
        }

        private static void ValidatePaths(IEnumerable<RingPoint[]> paths) {
            if (paths == null)
                throw new ArgumentNullException("paths");

            if (paths.Select(point => point.Length).Any(length => length < 2)) {
                throw new ArgumentException("Paths are made up of two or more points. Yours has less.");
            }
        }

        public void AddPath(List<RingPoint[]> paths) {
            ValidatePaths(paths);

            foreach (var path in paths) {
                Paths.Add(path);
            }
        }

        public override string Type => "polyline";
    }
}
