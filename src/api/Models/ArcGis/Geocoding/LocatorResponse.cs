namespace AGRC.api.Models.ArcGis;
public record LocatorResponse(List<LocatorCandidate> Candidates, RestEndpointError? Error)
    : RestErrorable(Error);

public record LocatorCandidate(string Address, Point Location, double Score, OutFields Attributes);

public record OutFields(string Addr_type, string Addnum);
