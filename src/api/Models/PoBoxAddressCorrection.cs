namespace api.mapserv.utah.gov.Models {
    public class PoBoxAddressCorrection : PoBoxAddress {
        public PoBoxAddressCorrection(int zip, int zip9, decimal x, decimal y) : base(zip, x, y) {
            ZipPlusFour = zip9;
        }

        public int ZipPlusFour { get; set; }
    }
}
