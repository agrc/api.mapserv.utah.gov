using System;

namespace AGRC.api.Models {
    public class PoBoxAddress {
        public PoBoxAddress(int zip, decimal x, decimal y) {
            Zip = zip;
            X = Convert.ToDouble(x);
            Y = Convert.ToDouble(y);
        }

        public int Zip { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
