namespace AGRC.api.Features.Geocoding;
public record PoBoxAddressCorrection(int ZipPlusFour, int Zip, double X, double Y) : PoBoxAddress(Zip, X, Y);
