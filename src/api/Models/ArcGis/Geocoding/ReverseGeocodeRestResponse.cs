#nullable enable
namespace AGRC.api.Models.ArcGis;
public record ReverseGeocodeRestResponse(ReverseMatchAddress? Address, Point? Location, RestEndpointError? Error) : RestErrorable(Error);
public record ReverseMatchAddress(string Address, string City, string Addr_type);
