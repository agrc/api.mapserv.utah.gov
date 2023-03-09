namespace AGRC.api.Features.Geocoding {
    public class PoBoxAddressCorrection : PoBoxAddress {
        public PoBoxAddressCorrection(int zip, int zip9, double x, double y) : base(zip, x, y) {
            ZipPlusFour = zip9;
        }

        public int ZipPlusFour { get; set; }
    }
}
