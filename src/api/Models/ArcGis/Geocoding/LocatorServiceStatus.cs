namespace AGRC.api.Models.ArcGis;
public record LocatorServiceStatus(double CurrentVersion, string Capabilities, RestEndpointError? RestEndpointError)
    : RestErrorable(RestEndpointError);
