using WebAPI.Domain.Addresses;

namespace WebAPI.Domain
{
    public class GeocodeOperationInput
    {
        public GeocodeOperationInput(GeocodeAddress addressInfo, string grid, int weight, LocatorDetails locator,
                            int wkId = 26912)
        {
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
        public LocatorDetails Locator { get; set; }
        public int WkId { get; set; }

        public override string ToString()
        {
            return string.Format("GeocodeInput: Address: {0}, Grid: {1}, Weight: {2}, Locator: {3}, WkId: {4}", Address,
                                 Grid, Weight, Locator, WkId);
        }
    }
}