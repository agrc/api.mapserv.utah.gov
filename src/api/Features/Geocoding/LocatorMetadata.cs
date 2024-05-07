namespace ugrc.api.Features.Geocoding;
public record LocatorMetadata(Address AddressInfo, string Grid, int Weight, LocatorProperties? Locator,
                        int WkId = 26912) {
    public string Address => AddressInfo.StandardizedAddress();
}
