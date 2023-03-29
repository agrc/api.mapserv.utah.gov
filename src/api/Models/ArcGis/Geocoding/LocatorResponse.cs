#nullable enable
namespace AGRC.api.Models.ArcGis;
public record LocatorResponse(List<Candidate> Candidates, RestEndpointError? Error)
    : RestErrorable(Error);
