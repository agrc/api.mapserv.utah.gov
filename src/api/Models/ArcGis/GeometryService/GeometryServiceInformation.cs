#nullable enable
namespace AGRC.api.Models.ArcGis;
public record GeometryServiceInformation(string ServiceDescription, RestEndpointError? Error) : RestErrorable(Error);
