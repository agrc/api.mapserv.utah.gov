namespace AGRC.api.Features.Geocoding;
public record LocatorMetadata(AddressWithGrids AddressInfo, string Grid, int Weight, LocatorProperties? Locator,
                        int WkId = 26912) {
    public string Address => AddressInfo.StandardizedAddress;
}
