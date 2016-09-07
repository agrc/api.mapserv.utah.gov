namespace WebAPI.Domain
{
    public class PoBoxAddress
    {
        public int Zip { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PoBoxAddress(int zip, double x, double y)
        {
            Zip = zip;
            X = x;
            Y = y;
        }
    }

    public class PoBoxAddressCorrection : PoBoxAddress
    {
        public int ZipPlusFour { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PoBoxAddressCorrection(int zip, int zip9, double x, double y) : base(zip, x, y)
        {
            ZipPlusFour = zip9;
        }
    }
        
}