using System;

namespace AGRC.api.Features.Geocoding {
    public class PoBoxAddress {
        public PoBoxAddress(int zip, double x, double y) {
            Zip = zip;
            X = x;
            Y = y;
        }

        public int Zip { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
