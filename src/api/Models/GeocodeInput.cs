namespace api.mapserv.utah.gov.Models {
    public class GeocodeInput {
        public GeocodeInput(GeocodeAddress addressInfo, string grid, int weight, LocatorProperties locator,
                            int wkId = 26912) {
            var street = addressInfo.StandardizedAddress;

            AddressInfo = addressInfo;
            Address = street;
            Grid = grid;
            Weight = weight;
            Locator = locator;
            WkId = wkId;
        }

        public GeocodeAddress AddressInfo { get; set; }
        public string Address { get; set; }
        public string Grid { get; set; }
        public int Weight { get; set; }
        public LocatorProperties Locator { get; set; }
        public int WkId { get; set; }
    }
}
