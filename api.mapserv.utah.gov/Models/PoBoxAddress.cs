using System;

namespace api.mapserv.utah.gov.Models
{
    public class PoBoxAddress
    {
        public int Zip { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PoBoxAddress(int zip, decimal x, decimal y)
        {
            Zip = zip;
            X = Convert.ToDouble(x);
            Y = Convert.ToDouble(y);
        }
    }
}